using DataMapper.RepoInterfaces;
using DomainModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for extension management operations with business logic validation
    /// </summary>
    public class ExtensionService : IExtensionService
    {
        private readonly IExtensionRepository _extensionRepository;
        private readonly IBorrowedBooksRepository _borrowedBooksRepository;
        private readonly ILogger<ExtensionService> _logger;

        /// <summary>
        /// Initializes a new instance of the ExtensionService class
        /// </summary>
        /// <param name="extensionRepository">Extension repository</param>
        /// <param name="borrowedBooksRepository">Borrowed books repository</param>
        /// <param name="logger">Logger instance</param> 
        public ExtensionService(
            IExtensionRepository extensionRepository,
            IBorrowedBooksRepository borrowedBooksRepository,
            ILogger<ExtensionService> logger)
        {
            _extensionRepository = extensionRepository ?? throw new ArgumentNullException(nameof(extensionRepository));
            _borrowedBooksRepository = borrowedBooksRepository ?? throw new ArgumentNullException(nameof(borrowedBooksRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Validate and pass an extension object to the data service for creation
        /// </summary>
        /// <param name="extension">The extension</param>
        public void AddExtension(Extension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension), "Extension cannot be null");

            ValidateExtension(extension);

            var borrowedBook = _borrowedBooksRepository.GetByIds(extension.BookId, extension.ReaderId);
            if (borrowedBook == null)
                throw new InvalidOperationException("Borrowed book record not found");

            _extensionRepository.Add(extension);
        }

        /// <summary>
        /// Validate and pass an extension object to the data service for updating
        /// </summary>
        /// <param name="extension">The extension</param>
        public void UpdateExtension(Extension extension)
        {
            try
            {
                if (extension == null)
                    throw new ArgumentNullException(nameof(extension), "Extension cannot be null");
                var existingExtension = GetExtensionById(extension.ExtensionId);
                if (existingExtension == null)
                    throw new InvalidOperationException($"Extension with ID {extension.ExtensionId} not found");
                ValidateExtension(extension);

                _extensionRepository.Update(extension);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding extension for BookId: {BookId}, ReaderId: {ReaderId}",
                    extension?.BookId, extension?.ReaderId);
                throw;
            }
        }

        /// <summary>
        /// Get an extension by its identifier
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        /// <returns>The extension if found</returns>
        public Extension GetExtensionById(int extensionId)
        {
            try
            {
                return _extensionRepository.GetById(extensionId);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving extension with ID: {ExtensionId}", extensionId);
                throw;
            }
        }

        /// <summary>
        /// Get all extensions
        /// </summary>
        /// <returns>List of all extensions</returns>
        public IList<Extension> GetAllExtensions()
        {
            try
            {
                return _extensionRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all extensions");
                throw;
            }
        }

        /// <summary>
        /// Delete an extension by its identifier
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        public void DeleteExtension(int extensionId)
        {
            try
            {
                var extension = GetExtensionById(extensionId);
                if (extension == null)
                    throw new InvalidOperationException($"Extension with ID {extensionId} not found");

                _extensionRepository.Delete(extensionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting extension with ID: {ExtensionId}", extensionId);
                throw;
            }
        }

        /// <summary>
        /// Get all extensions for a specific reader
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of extensions for the reader</returns>
        public IList<Extension> GetByReaderId(int readerId)
        {
            try
            {
                return _extensionRepository.GetByReaderId(readerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting extension with ID: {ReaderId}", readerId);
                throw;
            }
        }

        /// <summary>
        /// Get all extensions for a specific book
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>List of extensions for the book</returns>
        public IList<Extension> GetByBookId(int bookId)
        {
            return _extensionRepository.GetByBookId(bookId);
        }

        /// <summary>
        /// Get the total extension days for a reader in the last specified number of months
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="months">Number of months to look back</param>
        /// <returns>Total extension days</returns>
        public int GetTotalExtensionDaysForReaderInLastMonths(int readerId, int months)
        {
            if (months < 1)
                throw new ArgumentException("Months must be at least 1");

            return _extensionRepository.GetTotalExtensionDaysForReaderInLastMonths(readerId, months);
        }

        /// <summary>
        /// Validates extension data according to business rules
        /// </summary>
        /// <param name="extension">Extension to validate</param>
        private void ValidateExtension(Extension extension)
        {
            if (extension.BookId <= 0)
                throw new ArgumentException("Book ID must be positive");

            if (extension.ReaderId <= 0)
                throw new ArgumentException("Reader ID must be positive");

            if (extension.ExtensionDays < 1 || extension.ExtensionDays > 90)
                throw new ArgumentException("Extension days must be between 1 and 90");

            if (extension.RequestDate == DateTime.MinValue)
                throw new ArgumentException("Request date is required");

            if (extension.BorrowedBooks == null)
                throw new ArgumentException("Extension must be associated with a borrowed book");
        }
    }
}
