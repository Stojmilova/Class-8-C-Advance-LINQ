using BooksProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowAuthorsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            BooksLoader loader = new BooksLoader();
            var authors = loader.GetAllAuthors();

            /*Regular:
            1.What is the average number of books per autors?
            2.Which book(s) has the longest title, and how long is it?
            3.Which author has the shortest average title for a book?
            4.Which author has the shortest average title for a book? (Discount authors with less than three books)
            5.What series has the most books?
            6.Which year has the most books published?
            7.What is the average number of books published for years in the 21st centrury? (Starting with 2001, not 2000)
            8.Which author has the most different series?
            9.Which author has the most books written that belong to a series?
            10.Which author has the longest career? */

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***1.The average number of books per autors:");

            var averageBook = authors.Select(author => new
            {
                author.ID,
                author.Name,
                AverageBook = authors.Average(a => a.Books.Count())

            }); 
            Console.WriteLine(averageBook.First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***2.Book(s) with the longest title:");

            var allBooks = authors
                .SelectMany(author => author.Books)
                .OrderByDescending(book => book.Title.Length)
                .ThenBy(book => book.ID);

            var first = allBooks.First();
            Console.WriteLine($" ID:{first.ID} => Title:{first.Title} => Length:{first.Title.Count()}");

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***3.Author who has the shortest average title for a book:");

            var shortestTitle = authors.Select(author => new
            {
                author.ID,
                author.Name,
                ShortTitle = author.Books.Average(book => book.Title.Length)

            }).OrderBy(book => book.ShortTitle).ToList();

            Console.WriteLine(shortestTitle.First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***4.Author who has the shortest average title for a book:\n " 
                "(Discount authors with less than three books)");

            var shortestAverage = authors
                .Where(a => a.Books.Count() > 3)
                .Select(a => new
                {
                    a.ID,
                    a.Name,
                    ShortestAverage = a.Books.Average(b => b.Title.Length)
                }).OrderBy(b => b.ShortestAverage);

            Console.WriteLine(shortestAverage.First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***5.What series has the most books:");

            var authorsByLetter = authors
               .SelectMany(author => author.Books)
               .GroupBy(book => book.Series)
               .Select(book => new { book.Key, Count = book.Count() })
               .OrderByDescending(book => book.Count);

            Console.WriteLine(authorsByLetter.First());//First is a empty string;
            Console.WriteLine(authorsByLetter.Skip(1).First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***6.Which year has the most books published:");

            var mostPublishedYear = authors
                .SelectMany(author => author.Books)
                .GroupBy(book => book.Year)
                .OrderByDescending(book => book.Count());

            var firstYear = mostPublishedYear.First();
            Console.WriteLine($"In {firstYear.Key} year has the most published books: {firstYear.Count()} books");

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***7.The average number of books published for years in the 21st centrury?\n  (Starting with 2001, not 2000):");

            var century21Books = authors
                .SelectMany(author => author.Books)
                .Where(book => book.Year > 2000)
                .GroupBy(book => book.Year)
                .Average(book => book.Count());

            Console.WriteLine($"The average number of books published in the 21st century is: {century21Books} books");

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***8.Author who has the most different series:");

            var authorWithDiffSeries = authors.Select(a => new
            {
                a.ID,
                a.Name,
                NumbersOfDiffSeries = a.Books.GroupBy(b => b.Series).Distinct().Count()
            }).OrderByDescending(b => b.NumbersOfDiffSeries).ThenBy(b => b.Name);

            Console.WriteLine(authorWithDiffSeries.First());//There are two authors with the same number od different series;
            Console.WriteLine(authorWithDiffSeries.Skip(1).First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***9.Author who has the most books written that belong to a series:");

            var booksWithSeries = authors.Select(author => new
            {
                author.ID,
                author.Name,
                BookWithSerie = author.Books.Where(book => !string.IsNullOrEmpty(book.Series)).Count()
            }).OrderByDescending(author => author.BookWithSerie);

            Console.WriteLine(booksWithSeries.First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***10.Author with the longest career:");

            var longestCarrer = authors.Select(a => new
            {
                a.ID,
                a.Name,
                Sub = a.Books.Select(b => b.Year).Max() - a.Books.Select(b => b.Year).Min()
            }).OrderByDescending(a => a.Sub);

            Console.WriteLine(longestCarrer.First());


            //Bonus


            /*1.What series has the most authors ?
            2.In Which year most authors published a book?
            3.Which author has the highest average books per year?
            4.How long is the longest hiatus between two books for an author, and by whom ?*/

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***1.What series has the most authors:");

            var mostAuthorsSeries = authors
                .SelectMany(author => author.Books)
                .Select(book => new
                {
                    book.Title,
                    Author = authors.Where(author => author.Books.Contains(book)).First().Name

                }).GroupBy(book => book.Title).OrderByDescending(book => book.Count());

            var firstSeries = mostAuthorsSeries.First();
            Console.WriteLine($" Series: {firstSeries.Key} => NumberOfAuthors:{firstSeries.Count()}");

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***2.In Which year most authors published a book:");

            var yearMostPublished = authors
                 .SelectMany(author => author.Books)
                 .GroupBy(book => book.Year)
                 .Select(year => year.Key)
                 .Select(year => new
                 {
                     Year = year,
                     NumberOfAuthors = authors.Select(author => new
                     {
                         Years = author.Books.Select(book => book.Year).Distinct()

                     }).Where(a => a.Years.Contains(year))
                 }).OrderByDescending(x => x.NumberOfAuthors.Count());

            var firstMostPublishedYear = yearMostPublished.First();
            Console.WriteLine($"{firstMostPublishedYear.Year}Year => {firstMostPublishedYear.NumberOfAuthors.Count()} authors ");


            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***3.Which author has the highest average books per year:");

            var highestAverageBooks = authors.Select(author => new
            {
                author.ID,
                author.Name,
                BooksInYear = author.Books.GroupBy(book => book.Year).Average(book => book.Count())
            }).OrderByDescending(author => author.BooksInYear);

            Console.WriteLine(highestAverageBooks.First());

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("***4.How long is the longest hiatus between two books for an author, and by whom:");


            Console.ReadLine();
        }
        

        static void PrintAuthors<T>(IEnumerable<T> authors)
        {
            foreach (var author in authors)
            {
                Console.WriteLine(author);
            }
            Console.WriteLine("---------------");
        }
    }
}
