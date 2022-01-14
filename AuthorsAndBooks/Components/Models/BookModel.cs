using AuthorsAndBooks.Components.ResourceModels;
using System;

namespace AuthorsAndBooks.Models
{
    public class BookModel
    {
        public BookModel() { }

        public BookModel(AuthorModel author, BookResourceModel book)
        {
            Author = author;
            Name = book.Name;
            ReleaseDate = book.ReleaseDate;
        }

        public int Id { get; set; }

        public int AuthorId { get; set; }

        public AuthorModel Author { get; set; }

        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}