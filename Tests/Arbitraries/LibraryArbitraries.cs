using FsCheck;
using FsCheck.Fluent;
using Library_Book_Borrowing_System.Domain;
using System.Linq;

namespace Library_Book_Borrowing_System.Tests.Arbitraries
{
    public static class LibraryArbitraries
    {
        private static Gen<string> DigitsGen(int length)
            => Gen.ArrayOf(Gen.Choose(0, 9), length)
                  .Select(digits => string.Concat(digits));

        public static Arbitrary<string> Isbn()
            => Arb.From(Gen.OneOf(DigitsGen(10), DigitsGen(13)));

        private static Gen<string> Title()
            => Gen.Elements(
                "Clean Code",
                "Design Patterns",
                "Refactoring",
                "Domain-Driven Design",
                "The Pragmatic Programmer",
                "Test-Driven Development"
            );

        public static Arbitrary<Book> Book()
        {
            var gen = from isbn in Isbn().Generator
                      from title in Title()
                      from copies in Gen.Choose(1, 20)
                      select new Book(isbn, title, copies);

            return Arb.From(gen);
        }

        public static Arbitrary<User> User()
        {
            var names = Gen.Elements("Олена", "Максим", "Ірина", "Дмитро", "Софія", "Артем");
            var gen = from name in names
                      select new User(name);

            return Arb.From(gen);
        }
    }
}
