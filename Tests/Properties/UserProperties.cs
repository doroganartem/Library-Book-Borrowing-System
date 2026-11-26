using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class UserProperties
    {
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Service_borrow_and_return_restores_initial_state(Book book, User user)
        {
            var repo = new InMemoryLibraryRepository();
            repo.AddBook(book);
            repo.AddUser(user);
            var service = new LibraryService(repo);

            var initialCopies = book.AvailableCopies;

            service.BorrowBook(user.Id, book.Isbn);
            user.BorrowedIsbns.Should().Contain(book.Isbn);

            service.ReturnBook(user.Id, book.Isbn);

            book.AvailableCopies.Should().Be(initialCopies);
            user.BorrowedIsbns.Should().BeEmpty();
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Service_borrow_matches_direct_domain_borrow(Book book, User user)
        {
            var directBook = new Book(book.Isbn, book.Title, book.TotalCopies);

            var repo = new InMemoryLibraryRepository();
            repo.AddBook(book);
            repo.AddUser(user);
            var service = new LibraryService(repo);

            service.BorrowBook(user.Id, book.Isbn);
            directBook.Borrow();

            book.AvailableCopies.Should().Be(directBook.AvailableCopies);
        }
    }
}