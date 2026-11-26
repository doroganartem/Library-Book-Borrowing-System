using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;
using System;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class ErrorProperties
    {
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Creating_book_with_non_positive_copies_throws(string isbn, NonNegativeInt invalidCopies)
        {
            int copies = -invalidCopies.Get; 
            Action act = () => new Book(isbn, "Test Title", copies);
            act.Should().Throw<ArgumentException>();
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Borrowing_when_no_copies_available_throws(Book book)
        {
            while (book.AvailableCopies > 0)
                book.Borrow();

            Action act = () => book.Borrow();
            act.Should().Throw<InvalidOperationException>();
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Returning_when_all_copies_available_throws(Book book)
        {
            Action act = () => book.Return();
            act.Should().Throw<InvalidOperationException>();
        }


        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Returning_book_not_borrowed_by_user_throws(Book book, User user)
        {
            var repo = new InMemoryLibraryRepository();
            repo.AddBook(book);
            repo.AddUser(user);
            var service = new LibraryService(repo);

            Action act = () => service.ReturnBook(user.Id, book.Isbn);
            act.Should().Throw<InvalidOperationException>();
        }
    }
}