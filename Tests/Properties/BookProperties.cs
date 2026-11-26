using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;
using System;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class BookProperties
    {
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Borrow_then_return_restores_available_copies(Book book)
        {
            var initialAvailable = book.AvailableCopies;
            book.Borrow();
            book.Return();

            book.AvailableCopies.Should().Be(initialAvailable);
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public Property Two_sequential_borrows_equal_bulk_borrow(Book book)
        {
            bool hasAtLeastTwo = book.AvailableCopies >= 2;
            bool ok = true;

            if (hasAtLeastTwo)
            {
                var copy1 = new Book(book.Isbn, book.Title, book.TotalCopies);
                var copy2 = new Book(book.Isbn, book.Title, book.TotalCopies);


                copy1.Borrow();
                copy1.Borrow();

                for (int i = 0; i < 2; i++) copy2.Borrow();

                ok = copy1.AvailableCopies == copy2.AvailableCopies;
            }

            return ok.When(hasAtLeastTwo);
        }


        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public Property Borrowed_count_can_be_recovered_from_total_and_available(Book book)
        {

            return Prop.ForAll(Gen.Choose(1, 20).ToArbitrary(), count =>
            {
                bool applicable = count <= book.TotalCopies;
                bool ok = true;

                if (applicable)
                {
                    for (int i = 0; i < count; i++)
                        book.Borrow();
                    int recoveredBorrowed = book.TotalCopies - book.AvailableCopies;
                    ok = recoveredBorrowed == count;
                }
                return ok.When(applicable);
            });
        }

        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public void Borrow_on_newly_created_valid_book_never_throws(Book book)
        {
            Action act = () => book.Borrow();
            act.Should().NotThrow();
        }
    }
}
