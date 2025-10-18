using MediatR;

namespace Library.BusinessLayer.CQRS.Events;

public record BookCreatedEvent(int BookId, int AuthorId) : INotification;
