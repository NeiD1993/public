using AuthorsAndBooks.Components.Controllers;
using AuthorsAndBooks.Components.ResourceModels;
using AuthorsAndBooks.Components.Utils.ActionResults;
using AuthorsAndBooks.Models;
using AuthorsAndBooks.Utils.Attributes;
using AuthorsAndBooks.Utils.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorsAndBooks.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorsController : BaseAuthorsAndBooksController
    {
        public AuthorsController(AuthorsAndBooksDbContext dbContext) : base(dbContext) { }

        [HttpGet("{fullnamePrefix}")]
        public async Task<ActionResult<IEnumerable<AuthorResourceModel>>> Get(string fullnamePrefix)
        {
            return await dbContext.Authors.Where(author => author.Fullname.StartsWith(fullnamePrefix)).Select(author => new AuthorResourceModel(author)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<AuthorResourceModel>> Post(AuthorResourceModel author)
        {
            dbContext.Authors.Add(new AuthorModel(author));

            return await SaveDatabaseChangesAsync(author);
        }

        [HttpDelete]
        public async Task<ActionResult<AuthorResourceModel>> Delete([AuthorFullnameParts(1)] string surname, [AuthorFullnameParts(1)] string name, [AuthorFullnameParts(1)] string patronymic)
        {
            AuthorResourceModel authorResourceModel = new AuthorResourceModel(name, surname, patronymic);
            AuthorModel authorModel = await dbContext.Authors.FirstOrDefaultAsync(GetAuthorsFullnamesPartsComparator(authorResourceModel));

            if (authorModel == null)
                return new AuthorNotFoundObjectResult();
            else
            {
                dbContext.Authors.Remove(authorModel);

                return await SaveDatabaseChangesAsync(authorResourceModel);
            }
        }
    }
}