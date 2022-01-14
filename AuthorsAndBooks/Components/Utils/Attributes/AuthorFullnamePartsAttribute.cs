using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AuthorsAndBooks.Utils.Attributes
{
    public class AuthorFullnamePartsAttribute : RegularExpressionAttribute
    {
        public AuthorFullnamePartsAttribute(int partsCount) : base(string.Join(" ", Enumerable.Repeat(@"[A-Za-z](\.|([A-Za-z]+))", partsCount))) { }
    }
}