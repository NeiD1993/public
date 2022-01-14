using AuthorsAndBooks.Models;
using AuthorsAndBooks.Utils.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuthorsAndBooks.Components.ResourceModels
{
    public class BookResourceModel
    {
        public BookResourceModel() { }

        public BookResourceModel(string authorFullname, string name, DateTime releaseDate)
        {
            AuthorFullname = authorFullname;
            Name = name;
            ReleaseDate = releaseDate;
        }

        public BookResourceModel(BookModel book) : this(book.Author.Fullname, book.Name, book.ReleaseDate) { }

        [Required]
        [AuthorFullnameParts(3)]
        public string AuthorFullname { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
    }
}