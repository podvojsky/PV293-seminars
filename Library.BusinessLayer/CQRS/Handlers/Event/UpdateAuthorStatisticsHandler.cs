using Library.BusinessLayer.CQRS.Events;
using Library.DataAccess.Repositories;
using Library.DataAccess.UnitOfWork;
using MediatR;

namespace Library.BusinessLayer.CQRS.Handlers.Event;

public class UpdateAuthorStatisticsHandler(
    IAuthorRepository authorRepository,
    IBookRepository bookRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<BookCreatedEvent>
{
    public async Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(notification.AuthorId);
        if (author == null)
            throw new ArgumentException($"Author with ID {notification.AuthorId} not found", nameof(notification.AuthorId));

        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        var authorBooks = await bookRepository.GetAllAsync();
        var authorBooksList = authorBooks.Where(b => b.AuthorId == author.Id).ToList();
        if (authorBooksList.Count != 0)
        {
            var mostPopularGenre = authorBooksList
                .GroupBy(b => b.Genre)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(mostPopularGenre))
            {
                author.MostPopularGenre = mostPopularGenre;
            }
        }

        authorRepository.Update(author);

        // 5. Save all changes
        // NOTE: With MediatR pipeline behavior, this could be handled automatically
        // through a TransactionalBehavior that wraps all handlers in a transaction
        await unitOfWork.CompleteAsync();
    }
}
