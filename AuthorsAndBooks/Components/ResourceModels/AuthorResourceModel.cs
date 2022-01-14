using AuthorsAndBooks.Models;
using AuthorsAndBooks.Utils.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuthorsAndBooks.Components.ResourceModels
{
    public class AuthorResourceModel
    {
        public AuthorResourceModel() { }

        public AuthorResourceModel(string fullname)
        {
            string[] fullnameParts = fullname.Split(" ");

            InitializeFullnameParts(fullnameParts[1], fullnameParts[0], fullnameParts[2]);
        }

        public AuthorResourceModel(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }

        public AuthorResourceModel(AuthorModel author)
        {
            InitializeFullnameParts(author.Name, author.Surname, author.Patronymic);
            Birthdate = author.Birthdate;
        }

        [Required]
        [AuthorFullnameParts(1)]
        public string Name { get; set; }

        [Required]
        [AuthorFullnameParts(1)]
        public string Surname { get; set; }

        [Required]
        [AuthorFullnameParts(1)]
        public string Patronymic { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        private void InitializeFullnameParts(string name, string surname, string patronymic)
        {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
        }
    }
}