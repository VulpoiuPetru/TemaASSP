using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for author management operations with business logic validation
    /// </summary>
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        /// <summary>
        /// Initializes a new instance of the AuthorService class
        /// </summary>
        /// <param name="authorRepository">Author repository</param>
        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        }

        /// <summary>
        /// Validate and pass an author object to the data service for creation
        /// </summary>
        /// <param name="author">The author</param>
        public void AddAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author), "Author cannot be null");

            ValidateAuthor(author);

            _authorRepository.Add(author);
            _authorRepository.SaveChanges();
        }

        /// <summary>
        /// Validate and pass an author object to the data service for updating
        /// </summary>
        /// <param name="author">The author</param>
        public void UpdateAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author), "Author cannot be null");

            var existingAuthor = GetAuthorById(author.AuthorId);
            if (existingAuthor == null)
                throw new InvalidOperationException($"Author with ID {author.AuthorId} not found");

            ValidateAuthor(author);

            _authorRepository.Update(author);
            _authorRepository.SaveChanges();
        }

        /// <summary>
        /// Get an author by their identifier
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>The author if found</returns>
        public Author GetAuthorById(int authorId)
        {
            return _authorRepository.GetById(authorId);
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of all authors</returns>
        public IList<Author> GetAllAuthors()
        {
            return _authorRepository.GetAll();
        }

        /// <summary>
        /// Delete an author by their identifier
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        public void DeleteAuthor(int authorId)
        {
            var author = GetAuthorById(authorId);
            if (author == null)
                throw new InvalidOperationException($"Author with ID {authorId} not found");

            if (author.Books?.Any() == true)
                throw new InvalidOperationException("Cannot delete author with associated books");

            _authorRepository.Delete(authorId);
            _authorRepository.SaveChanges();
        }

        /// <summary>
        /// Validates author data according to business rules
        /// </summary>
        /// <param name="author">Author to validate</param>
        private void ValidateAuthor(Author author)
        {
            if (string.IsNullOrWhiteSpace(author.FirstName))
                throw new ArgumentException("First name is required");

            if (author.FirstName.Length < 5 || author.FirstName.Length > 50)
                throw new ArgumentException("First name must be between 5 and 50 characters");

            if (string.IsNullOrWhiteSpace(author.LastName))
                throw new ArgumentException("Last name is required");

            if (author.LastName.Length < 5 || author.LastName.Length > 50)
                throw new ArgumentException("Last name must be between 5 and 50 characters");

            if (author.Age < 10 || author.Age > 80)
                throw new ArgumentException("Age must be between 10 and 80");
        }
    }
}
