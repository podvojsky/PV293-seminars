using FluentValidation;
using Library.BusinessLayer.CQRS.Commands;

namespace Library.BusinessLayer.CQRS.Validators;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.BookDto.Title)
            .NotEmpty().WithMessage("Book title is required");

        RuleFor(x => x.BookDto.AuthorId)
            .GreaterThan(0).WithMessage("Valid author ID is required");

        RuleFor(x => x.BookDto.ISBN)
            .NotEmpty().WithMessage("ISBN is required");
    }
}
