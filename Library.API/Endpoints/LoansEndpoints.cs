using System.Security.Claims;
using Library.Application.Loans.Commands;
using Library.Domain.Constants;
using Library.Domain.ValueObjects;
using MediatR;

namespace Library.API.Endpoints;

public static class LoansEndpoints
{
    public static void MapLoansEndpoints(this WebApplication app)
    {
        var loans = app.MapGroup("/api/loans")
            .WithTags("Loans")
            .WithOpenApi();

        loans.MapPost("/", CheckOutBook)
            .WithName("CheckOutBook")
            .Accepts<CheckOutBookRequest>("application/json")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy
                .RequireRole(UserRoles.Member, UserRoles.Librarian, UserRoles.Admin));

        loans.MapPost("/{loanId}/report-damage", ReportDamage).WithName("ReportDamage")
            .Accepts<ReportDamageRequest>("application/json")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy
                .RequireRole(UserRoles.Member, UserRoles.Librarian, UserRoles.Admin));
    }

    private static async Task<IResult> CheckOutBook(
        CheckOutBookRequest request,
        IMediator mediator,
        ClaimsPrincipal user)
    {
        var command = new CheckOutBookCommand
        {
            BookId = request.BookId,
            BorrowerId = request.BorrowerId,
            LoanDurationDays = request.LoanDurationDays ?? 14,
            User = user
        };

        var loanId = await mediator.Send(command);
        return Results.Created($"/api/loans/{loanId}", new { id = loanId });
    }

    private static async Task<IResult> ReportDamage(
        Guid loanId,
        ReportDamageRequest request,
        IMediator mediator,
        ClaimsPrincipal user)
    {
        var command = new ReportDamageCommand
        {
            LoanId = loanId,
            Description = request.Description,
            Cost = request.Cost
        };

        await mediator.Send(command);
        return Results.Ok();
    }
}

public record CheckOutBookRequest
{
    public Guid BookId { get; init; }
    public Guid? BorrowerId { get; init; }
    public int? LoanDurationDays { get; init; }
}

public record ReportDamageRequest
{
    public string Description { get; init; }

    public Money Cost { get; init; }
}
