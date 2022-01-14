using Microsoft.AspNetCore.Mvc;

namespace AuthorsAndBooks.Components.Utils.ActionResults
{
    public abstract class AuthorOrBookNotFoundObjectResult : NotFoundObjectResult
    {
        public AuthorOrBookNotFoundObjectResult(string messagePrefix) : base(messagePrefix + " is not found") { }
    }
}