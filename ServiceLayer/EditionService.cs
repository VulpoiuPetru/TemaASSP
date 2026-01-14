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
    /// Service for edition management operations with business logic validation
    /// </summary>
    public class EditionService : IEditionService
    {
        private readonly IEditionRepository _editionRepository;
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<EditionService> _logger;
        private readonly IValidator<Edition> _validator;

        /// <summary>
        /// Initializes a new instance of the EditionService class
        /// </summary>
        /// <param name="editionRepository">Edition repository</param>
        /// <param name="bookRepository">Book repository</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="validator">FluentValidation validator for Edition</param>
        public EditionService(IEditionRepository editionRepository, IBookRepository bookRepository, ILogger<EditionService> logger, IValidator<Edition> validator)
        {
            _editionRepository = editionRepository ?? throw new ArgumentNullException(nameof(editionRepository));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Validate and pass an edition object to the data service for creation
        /// </summary>
        /// <param name="edition">The edition</param>
        public void AddEdition(Edition edition)
        {
            try
            {
                if (edition == null)
                    throw new ArgumentNullException(nameof(edition), "Edition cannot be null");

                ValidateEdition(edition);

                if (edition.Book != null)
                {
                    var book = _bookRepository.GetById(edition.Book.BookId);
                    if (book == null)
                        throw new InvalidOperationException($"Book with ID {edition.Book.BookId} not found");
                }

                _editionRepository.Add(edition);
                _editionRepository.SaveChanges();
                _logger.LogInformation("Edition added successfully: Publisher {Publisher}, Year {Year}", edition.Publisher, edition.YearOfPublishing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding edition: Publisher: {Publisher}, Year: {Year}",
                    edition?.Publisher, edition?.YearOfPublishing);
                throw;
            }
        }

        /// <summary>
        /// Validate and pass an edition object to the data service for updating
        /// </summary>
        /// <param name="edition">The edition</param>
        public void UpdateEdition(Edition edition)
        {
            try
            {
                if (edition == null)
                    throw new ArgumentNullException(nameof(edition), "Edition cannot be null");

                var existingEdition = GetEditionById(edition.EditionId);
                if (existingEdition == null)
                    throw new InvalidOperationException($"Edition with ID {edition.EditionId} not found");

                ValidateEdition(edition);

                _editionRepository.Update(edition);
                _editionRepository.SaveChanges();

                _logger.LogInformation("Edition updated successfully: ID {EditionId}", edition.EditionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating edition with ID: {EditionId}", edition?.EditionId);
                throw;
            }
        }

        /// <summary>
        /// Get an edition by its identifier
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>The edition if found</returns>
        public Edition GetEditionById(int editionId)
        {
            try
            {
                return _editionRepository.GetById(editionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving edition with ID: {EditionId}", editionId);
                throw;
            }
        }

        /// <summary>
        /// Get all editions
        /// </summary>
        /// <returns>List of all editions</returns>
        public IList<Edition> GetAllEditions()
        {
            return _editionRepository.GetAll();
        }

        /// <summary>
        /// Delete an edition by its identifier
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        public void DeleteEdition(int editionId)
        {
            try
            {
                var edition = GetEditionById(editionId);
                if (edition == null)
                    throw new InvalidOperationException($"Edition with ID {editionId} not found");

                if (edition.Copies?.Any() == true)
                    throw new InvalidOperationException("Cannot delete edition with associated copies");

                _editionRepository.Delete(editionId);
                _editionRepository.SaveChanges();

                _logger.LogInformation("Edition deleted successfully: ID {EditionId}", editionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting edition with ID: {EditionId}", editionId);
                throw;
            }
        }

        /// <summary>
        /// Get editions by publisher name
        /// </summary>
        /// <param name="publisherName">The publisher name</param>
        /// <returns>List of editions from the publisher</returns>
        public IList<Edition> GetByPublisher(string publisherName)
        {
            if (string.IsNullOrWhiteSpace(publisherName))
                throw new ArgumentException("Publisher name cannot be empty");

            return _editionRepository.GetByPublisher(publisherName);
        }

        /// <summary>
        /// Get editions by year of publishing
        /// </summary>
        /// <param name="year">The year of publishing</param>
        /// <returns>List of editions from the year</returns>
        public IList<Edition> GetByYear(int year)
        {
            try
            {
                if (year < 1000 || year > 2999)
                    throw new ArgumentException("Year must be between 1000 and 2999");

                return _editionRepository.GetByYear(year);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving editions for year:",year);
                throw;
            }
        }

        /// <summary>
        /// Validates edition data according to business rules
        /// </summary>
        /// <param name="edition">Edition to validate</param>
        private void ValidateEdition(Edition edition)
        {
            if (string.IsNullOrWhiteSpace(edition.Publisher))
                throw new ArgumentException("Publisher cannot be empty");

            if (edition.Publisher.Length < 5)
                throw new ArgumentException("Publisher name must be at least 5 characters long");

            if (edition.NumberOfPages < 3)
                throw new ArgumentException("Number of pages must be at least 3");

            if (edition.YearOfPublishing < 1000 || edition.YearOfPublishing > 2999)
                throw new ArgumentException("Year must be between 1000 and 2999");

            if (string.IsNullOrWhiteSpace(edition.Type))
                throw new ArgumentException("Type cannot be empty");

            if (edition.Book == null)
                throw new ArgumentException("Edition must be associated with a book");

            var result = _validator.Validate(edition);
            if (!result.IsValid)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Edition validation failed: {errors}");
            }
        }


    }
}
