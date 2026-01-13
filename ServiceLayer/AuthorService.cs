using DataMapper.RepoInterfaces;
using DomainModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthorService> _logger;
        private readonly IValidator<Author> _validator;

        /// <summary>
        /// Initializes a new instance of the AuthorService class
        /// </summary>
        /// <param name="authorRepository">Author repository</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="validator">FluentValidation validator for Author</param>
        public AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger, IValidator<Author> validator)
        {
            _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Validate and pass an author object to the data service for creation
        /// </summary>
        /// <param name="author">The author</param>
        public void AddAuthor(Author author)
        {
            try
            {
                if (author == null)
                    throw new ArgumentNullException(nameof(author), "Author cannot be null");

                ValidateAuthor(author);

                _authorRepository.Add(author);
                _authorRepository.SaveChanges();

                _logger.LogInformation("Author added successfully: {FirstName} {LastName}", author.FirstName, author.LastName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding author: {FirstName} {LastName}", author?.FirstName, author?.LastName);
                throw;
            }
        }

        /// <summary>
        /// Validate and pass an author object to the data service for updating
        /// </summary>
        /// <param name="author">The author</param>
        public void UpdateAuthor(Author author)
        {
            try
            {
                if (author == null)
                    throw new ArgumentNullException(nameof(author), "Author cannot be null");

                var existingAuthor = GetAuthorById(author.AuthorId);
                if (existingAuthor == null)
                    throw new InvalidOperationException($"Author with ID {author.AuthorId} not found");

                ValidateAuthor(author);

                _authorRepository.Update(author);
                _authorRepository.SaveChanges();

                _logger.LogInformation("Author updated successfully: ID {AuthorId}", author.AuthorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating author with ID: {AuthorId}", author?.AuthorId);
                throw;
            }
        }

        /// <summary>
        /// Get an author by their identifier
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>The author if found</returns>
        public Author GetAuthorById(int authorId)
        {
            try
            {
                return _authorRepository.GetById(authorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving author with ID: {AuthorId}", authorId);
                throw;
            }
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of all authors</returns>
        public IList<Author> GetAllAuthors()
        {
            try
            {
                return _authorRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all authors");
                throw;
            }
        }

        /// <summary>
        /// Delete an author by their identifier
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        public void DeleteAuthor(int authorId)
        {
            try
            {
                var author = GetAuthorById(authorId);
                if (author == null)
                    throw new InvalidOperationException($"Author with ID {authorId} not found");

                if (author.Books?.Any() == true)
                    throw new InvalidOperationException("Cannot delete author with associated books");

                _authorRepository.Delete(authorId);
                _authorRepository.SaveChanges();

                _logger.LogInformation("Author deleted successfully: ID {AuthorId}", authorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting author with ID: {AuthorId}", authorId);
                throw;
            }
        }

        /// <summary>
        /// Validates author data according to business rules
        /// </summary>
        /// <param name="author">Author to validate</param>
        private void ValidateAuthor(Author author)
        {
            // Try FluentValidation first (if configured to return a result)
            if (_validator != null)
            {
                var result = _validator.Validate(author);
                if (result != null)
                {
                    if (!result.IsValid)
                    {
                        var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                        // Keep backward compatibility with tests that expect ArgumentException
                        throw new ArgumentException(errors);
                    }

                    // Valid according to validator
                    return;
                }
            }

            // Fallback manual validation to avoid NullReferenceException from misconfigured mocks
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
