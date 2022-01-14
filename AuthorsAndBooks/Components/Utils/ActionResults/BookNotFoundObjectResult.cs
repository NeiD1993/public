namespace AuthorsAndBooks.Components.Utils.ActionResults
{
    public class BookNotFoundObjectResult : AuthorOrBookNotFoundObjectResult
    {
        public BookNotFoundObjectResult() : base("A book") { }
    }
}