using Library.BusinessLayer.CQRS.Commands;
using Library.BusinessLayer.CQRS.Events;
using Library.BusinessLayer.CQRS.Queries;
using Library.BusinessLayer.Dtos;
using Library.BusinessLayer.Services;
using MediatR;

namespace Library.API.Endpoints;

public static class BooksEndpoints
{
    public static void MapBooksEndpoints(this WebApplication app)
    {
        var books = app.MapGroup("/api/books")
            .WithTags("Books")
            .WithOpenApi();

        books.MapGet("", GetBooks)
            .WithName("GetBooks")
            .Produces<List<BookDto>>();

        books.MapPost("", CreateBook)
            .WithName("CreateBook")
            .Accepts<BookDto>("application/json")
            .Produces<BookDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> GetBooks(ISender sender, CancellationToken cancellationToken)
    {
        var books = await sender.Send(new GetAllBooksQuery(), cancellationToken);
        return Results.Ok(books);
    }

    private static async Task<IResult> CreateBook(BookDto bookDto, IMediator mediator, CancellationToken cancellationToken)
    {
        try
        {
            var createdBook = await mediator.Send(new CreateBookCommand(bookDto), cancellationToken);
            return Results.Created($"/api/books/{createdBook.Id}", createdBook);
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { ex.ParamName ?? "Error", [ex.Message] },
            });
        }
    }
}
