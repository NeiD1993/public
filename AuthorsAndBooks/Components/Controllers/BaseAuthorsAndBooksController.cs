using AuthorsAndBooks.Components.ResourceModels;
using AuthorsAndBooks.Models;
using AuthorsAndBooks.Utils.Contexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthorsAndBooks.Components.Controllers
{
    public abstract class BaseAuthorsAndBooksController : ControllerBase
    {
        protected readonly AuthorsAndBooksDbContext dbContext;

        public BaseAuthorsAndBooksController(AuthorsAndBooksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected static Expression<Func<AuthorModel, bool>> GetAuthorsFullnamesPartsComparator(AuthorResourceModel authorResourceModel)
        {
            return authorModel => (authorModel.Name == authorResourceModel.Name) && (authorModel.Surname == authorResourceModel.Surname)
            && (authorModel.Patronymic == authorResourceModel.Patronymic);
        }

        protected IQueryable<BookModel> GetAuthorBooks(string authorFullname)
        {
            AuthorResourceModel authorResourceModel = new AuthorResourceModel(authorFullname);

            return dbContext.Books.Where(book => book.AuthorId == dbContext.Authors.First(GetAuthorsFullnamesPartsComparator(authorResourceModel)).Id);
        }

        protected async Task<ActionResult<T>> SaveDatabaseChangesAsync<T>(T obj)
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }

            return Ok(obj);
        }
    }
}