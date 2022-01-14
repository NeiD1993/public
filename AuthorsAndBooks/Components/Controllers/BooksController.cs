using AuthorsAndBooks.Components.ResourceModels;
using AuthorsAndBooks.Components.Utils.ActionResults;
using AuthorsAndBooks.Models;
using AuthorsAndBooks.Utils.Attributes;
using AuthorsAndBooks.Utils.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorsAndBooks.Components.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BooksController : BaseAuthorsAndBooksController
    {
        public BooksController(AuthorsAndBooksDbContext dbContext) : base(dbContext) { }

        [HttpGet("name-startsWith-{name}")]
        public async Task<ActionResult<IEnumerable<BookResourceModel>>> GetByName(string name)
        {
            return await dbContext.Books.Where(book => book.Name.StartsWith(name)).Include(book => book.Author).Select(book => new BookResourceModel(book)).ToListAsync();
        }

        [HttpGet("authorFullname-is-{authorFullname}")]
        public async Task<ActionResult<IEnumerable<BookResourceModel>>> GetByAuthorFullname([AuthorFullnameParts(3)] string authorFullname)
        {
            try
            {
                return await GetAuthorBooks(authorFullname).Select(book => new BookResourceModel(authorFullname, book.Name, book.ReleaseDate)).ToListAsync();
            }
            catch (Exception)
            {
                return new AuthorNotFoundObjectResult();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookResourceModel>> Post(BookResourceModel book)
        {
            AuthorModel author = await dbContext.Authors.FirstOrDefaultAsync(GetAuthorsFullnamesPartsComparator(new AuthorResourceModel(book.AuthorFullname)));

            if (author != null)
            {
                dbContext.Books.Add(new BookModel(author, book));

                return await SaveDatabaseChangesAsync(book);
            }

            return new AuthorNotFoundObjectResult();
        }

        [HttpDelete]
        public async Task<ActionResult<BookResourceModel>> Delete([AuthorFullnameParts(3)] string authorFullname, string name)
        {
            BookModel book;

            try
            {
                book = await GetAuthorBooks(authorFullname).FirstOrDefaultAsync(book => book.Name == name);
            }
            catch (Exception)
            {
                return new AuthorNotFoundObjectResult();
            }

            if (book == null)
                return new BookNotFoundObjectResult();

            dbContext.Books.Remove(book);

            return await SaveDatabaseChangesAsync(new BookResourceModel(authorFullname, name, book.ReleaseDate));
        }
    }
}