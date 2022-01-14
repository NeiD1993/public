using AuthorsAndBooks.Models;
using AuthorsAndBooks.Utils.Parsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthorsAndBooks.Utils.Contexts
{
    public class AuthorsAndBooksDbContext : DbContext
    {
        private static object databaseCreationLocker = new object();

        private readonly IConfiguration configuration;

        private readonly AuthorsAndBooksParser authorsAndBooksParser;

        public AuthorsAndBooksDbContext(IConfiguration configuration, AuthorsAndBooksParser authorsAndBooksParser)
        {
            this.configuration = configuration;
            this.authorsAndBooksParser = authorsAndBooksParser;

            lock (databaseCreationLocker)
            {
                if (Database.EnsureCreated())
                    Initialize();
            }
        }

        public DbSet<AuthorModel> Authors { get; set; }

        public DbSet<BookModel> Books { get; set; }

        private void Initialize()
        {            
            (AuthorModel[] authors, BookModel[] books) data = authorsAndBooksParser.Parse();

            Authors.AddRange(data.authors);
            Books.AddRange(data.books);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorModel>().HasAlternateKey(author => new { author.Name, author.Surname, author.Patronymic });
            modelBuilder.Entity<AuthorModel>().Property(author => author.Fullname).HasComputedColumnSql("[Surname] + ' ' + [Name] + ' ' + [Patronymic]");

            modelBuilder.Entity<BookModel>().HasAlternateKey(book => new { book.AuthorId, book.Name });
        }
    }
}