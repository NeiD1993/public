namespace AuthorsAndBooks.Components.Utils.ActionResults
{
    public class AuthorNotFoundObjectResult : AuthorOrBookNotFoundObjectResult
    {
        public AuthorNotFoundObjectResult() : base("An author") { }
    }
}