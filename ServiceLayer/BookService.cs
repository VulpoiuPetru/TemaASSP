using DataMapper.RepoInterfaces;
using DomainModel;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for book management operations with business logic validation
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IConfigurationService _configService;
        private readonly IBookRepository _bookRepository;
        private readonly IDomainRepository _domainRepository;
        private readonly ILogger<BookService> _logger;
        private readonly IValidator<Book> _validator;

        /// <summary>
        /// Initializes a new instance of the BookService class
        /// </summary>
        public BookService(
            IConfigurationService configService,
            IBookRepository bookRepository,
            IDomainRepository domainRepository,
            ILogger<BookService> logger,
            IValidator<Book> validator)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _domainRepository = domainRepository ?? throw new ArgumentNullException(nameof(domainRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Validate and pass a book object to the data service for creation
        /// </summary>
        public void AddBook(Book book)
        {
            try
            {
                if (book == null)
                    throw new ArgumentNullException(nameof(book), "Book cannot be null");

                ValidateBook(book);
                ValidateDomainAssignment(book);

                _bookRepository.Add(book);
                _bookRepository.SaveChanges();

                _logger.LogInformation("Book added successfully: {Title}", book.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book: {Title}", book?.Title);
                throw;
            }
        }

        /// <summary>
        /// Validate and pass a book object to the data service for updating
        /// </summary>
        public void UpdateBook(Book book)
        {
            try
            {
                if (book == null)
                    throw new ArgumentNullException(nameof(book), "Book cannot be null");

                var existingBook = GetBookById(book.BookId);
                if (existingBook == null)
                    throw new InvalidOperationException($"Book with ID {book.BookId} not found");

                ValidateBook(book);
                ValidateDomainAssignment(book);

                _bookRepository.Update(book);
                _bookRepository.SaveChanges();

                _logger.LogInformation("Book updated successfully: ID {BookId}", book.BookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book with ID: {BookId}", book?.BookId);
                throw;
            }
        }

        /// <summary>
        /// Get a book by its identifier
        /// </summary>
        public Book GetBookById(int bookId)
        {
            try
            {
                return _bookRepository.GetById(bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID: {BookId}", bookId);
                throw;
            }
        }

        /// <summary>
        /// Get all books in the library
        /// </summary>
        public IList<Book> GetAllBooks()
        {
            try
            {
                return _bookRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all books");
                throw;
            }
        }

        /// <summary>
        /// Delete a book by its identifier
        /// </summary>
        public void DeleteBook(int bookId)
        {
            try
            {
                var book = GetBookById(bookId);
                if (book == null)
                    throw new InvalidOperationException($"Book with ID {bookId} not found");

                if (book.BorrowedBooks?.Any(bb => bb.BorrowEndDate > DateTime.Now) == true)
                    throw new InvalidOperationException("Cannot delete book with active borrows");

                _bookRepository.Delete(bookId);
                _bookRepository.SaveChanges();

                _logger.LogInformation("Book deleted successfully: ID {BookId}", bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID: {BookId}", bookId);
                throw;
            }
        }

        /// <summary>
        /// Set domains for a book with validation
        /// </summary>
        public void SetBookDomains(int bookId, IList<int> domainIds)
        {
            try
            {
                var book = GetBookById(bookId);
                if (book == null)
                    throw new InvalidOperationException($"Book with ID {bookId} not found");

                if (domainIds == null || !domainIds.Any())
                    throw new ArgumentException("At least one domain must be specified");

                var config = _configService.GetConfiguration();

                if (domainIds.Count > config.DOMENII)
                    throw new InvalidOperationException($"Maximum allowed domains per book is {config.DOMENII}");

                ValidateDomainRelationships(domainIds);

                book.Domains.Clear();
                foreach (var domainId in domainIds)
                {
                    var domain = _domainRepository.GetById(domainId);
                    if (domain == null)
                        throw new InvalidOperationException($"Domain with ID {domainId} not found");

                    book.Domains.Add(domain);
                }

                _bookRepository.Update(book);
                _bookRepository.SaveChanges();

                _logger.LogInformation("Domains set for book ID {BookId}", bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting domains for book ID: {BookId}", bookId);
                throw;
            }
        }

        /// <summary>
        /// Validates book data according to business rules (FluentValidation)
        /// </summary>
        private void ValidateBook(Book book)
        {
            ValidationResult result = _validator.Validate(book);

            if (!result.IsValid)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Book validation failed: {errors}");
            }
        }

        /// <summary>
        /// Validates domain assignment according to business rules
        /// </summary>
        private void ValidateDomainAssignment(Book book)
        {
            var config = _configService.GetConfiguration();

            if (book.Domains.Count > config.DOMENII)
                throw new InvalidOperationException($"Maximum allowed domains per book is {config.DOMENII}");

            var domainIds = book.Domains.Select(d => d.DomainId).ToList();
            ValidateDomainRelationships(domainIds);
        }

        /// <summary>
        /// Validates that domains don't have ancestor-descendant relationships
        /// </summary>
        private void ValidateDomainRelationships(IList<int> domainIds)
        {
            for (int i = 0; i < domainIds.Count; i++)
            {
                for (int j = i + 1; j < domainIds.Count; j++)
                {
                    if (_domainRepository.IsAncestor(domainIds[i], domainIds[j]))
                    {
                        throw new InvalidOperationException(
                            $"Cannot assign book to both a domain and its ancestor domain (IDs: {domainIds[i]}, {domainIds[j]})");
                    }

                    if (_domainRepository.IsAncestor(domainIds[j], domainIds[i]))
                    {
                        throw new InvalidOperationException(
                            $"Cannot assign book to both a domain and its ancestor domain (IDs: {domainIds[j]}, {domainIds[i]})");
                    }
                }
            }
        }
    }
}
