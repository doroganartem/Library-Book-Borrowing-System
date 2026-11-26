using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Library_Book_Borrowing_System.Domain;
using Library_Book_Borrowing_System.Tests.Arbitraries;

namespace Library_Book_Borrowing_System.Tests.Properties
{
    public class InvariantProperties
    {
        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public Property Book_available_copies_always_within_bounds(Book book, bool[] actions)
        {
            foreach (var borrow in actions)
            {
                if (borrow && book.AvailableCopies > 0)
                    book.Borrow();
                else if (!borrow && book.AvailableCopies < book.TotalCopies)
                    book.Return();
            }

            bool ok = book.AvailableCopies >= 0 && book.AvailableCopies <= book.TotalCopies;
            return ok.ToProperty();
        }


        [Property(Arbitrary = new[] { typeof(LibraryArbitraries) })]
        public Property Library_conservation_of_copies_after_borrow_and_return(Book book, User user, PositiveInt borrowCount)
        {
            int count = borrowCount.Get;
            bool canBorrow = count <= book.TotalCopies;
            bool ok = true;

            if (canBorrow)
            {
                var repo = new InMemoryLibraryRepository();
                repo.AddBook(book);
                repo.AddUser(user);
                var service = new LibraryService(repo);

                for (int i = 0; i < count; i++)
                    service.BorrowBook(user.Id, book.Isbn);

                ok = (book.AvailableCopies + user.BorrowedIsbns.Count) == book.TotalCopies;
            }
            return ok.When(canBorrow);
        }
    }

}
