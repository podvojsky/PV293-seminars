using Library.Application.CQRS;
using Library.Application.RepositoryInterfaces;

namespace Library.Application.Books.Events;

public class BookCreatedEvent : IDomainEvent
{
    public Guid BookId { get; set; }
    public Guid AuthorId { get; set; }
    public string Genre { get; set; } = string.Empty;
}

public class BookCreatedEventHandler(IAuthorRepository authorRepository, IBookRepository bookRepository)
    : IDomainEventHandler<BookCreatedEvent>
{
    public async Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Update author statistics
        var author = await authorRepository.GetByIdAsync(notification.AuthorId);

        if (author == null)
            return;

        // Increment total books published
        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        // Update the author's most popular genre based on all their books
        var mostPopularGenre = await bookRepository.GetMostPopularGenre(notification.AuthorId);

        if (!string.IsNullOrEmpty(mostPopularGenre)) author.MostPopularGenre = mostPopularGenre;

        authorRepository.Update(author);
    }
}
