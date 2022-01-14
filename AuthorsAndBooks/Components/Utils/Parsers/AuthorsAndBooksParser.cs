using AuthorsAndBooks.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Xml.Linq;

namespace AuthorsAndBooks.Utils.Parsers
{
    public class AuthorsAndBooksParser
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public AuthorsAndBooksParser(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        private static string GetEntityName<T>()
        {
            string typeName = typeof(T).Name;

            return typeName.Remove(typeName.Length - "Model".Length);
        }

        private (XDocument document, T[] entities) PreParse<T>()
        {
            string entityName = GetEntityName<T>();
            XDocument document = XDocument.Load(Path.Combine(webHostEnvironment.ContentRootPath, "Resources", "XML", entityName + "s.xml"));

            return (document, new T[int.Parse(document.Element(entityName + "s").Attribute("amount").Value)]);
        }

        private T[] Parse<T>(Func<XElement, T> entityParser)
        {
            int iterationIndex = 0;
            string entityName = GetEntityName<T>();
            (XDocument document, T[] entities) = PreParse<T>();

            foreach (XElement authorElement in document.Element(entityName + "s").Elements(entityName))
                entities[iterationIndex++] = entityParser(authorElement);

            return entities;
        }

        public (AuthorModel[] authors, BookModel[] books) Parse()
        {
            AuthorModel[] authors = Parse(authorElement => new AuthorModel()
            {
                Name = authorElement.Element("Name").Value,
                Surname = authorElement.Element("Surname").Value,
                Patronymic = authorElement.Element("Patronymic").Value
            });

            return (authors, Parse(bookElement => new BookModel()
            {
                Author = authors[int.Parse(bookElement.Element("AuthorIndex").Value) - 1],
                Name = bookElement.Element("Name").Value
            }));
        }
    }
}