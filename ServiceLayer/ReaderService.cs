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
    /// Service for reader management operations with business logic validation
    /// </summary>
    public class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;

        /// <summary>
        /// Initializes a new instance of the ReaderService class
        /// </summary>
        /// <param name="readerRepository">Reader repository</param>
        public ReaderService(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository ?? throw new ArgumentNullException(nameof(readerRepository));
        }

        /// <summary>
        /// Validate and pass a reader object to the data service for creation
        /// </summary>
        /// <param name="reader">The reader</param>
        public void AddReader(Reader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), "Reader cannot be null");

            // Validate reader data
            ValidateReader(reader);

            // Check for duplicate email/phone (business rule)
            ValidateUniqueContact(reader);

            // Add to database
            _readerRepository.Add(reader);
            _readerRepository.SaveChanges();
        }

        /// <summary>
        /// Validate and pass a reader object to the data service for updating
        /// </summary>
        /// <param name="reader">The reader</param>
        public void UpdateReader(Reader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), "Reader cannot be null");

            var existingReader = GetReaderById(reader.ReaderId);
            if (existingReader == null)
                throw new InvalidOperationException($"Reader with ID {reader.ReaderId} not found");

            // Validate updated reader data
            ValidateReader(reader);

            // Check for duplicate contact info (excluding self)
            ValidateUniqueContact(reader, excludeReaderId: reader.ReaderId);

            // Update in database
            _readerRepository.Update(reader);
            _readerRepository.SaveChanges();
        }

        /// <summary>
        /// Get a reader by their identifier
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The reader if found</returns>
        public Reader GetReaderById(int readerId)
        {
            return _readerRepository.GetById(readerId);
        }

        /// <summary>
        /// Get all readers in the library
        /// </summary>
        /// <returns>List of all readers</returns>
        public IList<Reader> GetAllReaders()
        {
            return _readerRepository.GetAll();
        }

        /// <summary>
        /// Delete a reader by their identifier
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        public void DeleteReader(int readerId)
        {
            var reader = GetReaderById(readerId);
            if (reader == null)
                throw new InvalidOperationException($"Reader with ID {readerId} not found");

            // Check if reader has active borrows (business rule)
            if (reader.BorrowedBooks?.Any(bb => bb.BorrowEndDate > DateTime.Now) == true)
                throw new InvalidOperationException("Cannot delete reader with active borrows");

            _readerRepository.Delete(readerId);
            _readerRepository.SaveChanges();
        }

        /// <summary>
        /// Validates reader data according to business rules
        /// </summary>
        /// <param name="reader">Reader to validate</param>
        private void ValidateReader(Reader reader)
        {
            if (string.IsNullOrWhiteSpace(reader.FirstName))
                throw new ArgumentException("First name is required");

            if (string.IsNullOrWhiteSpace(reader.LastName))
                throw new ArgumentException("Last name is required");

            if (reader.Age < 10 || reader.Age > 80)
                throw new ArgumentException("Age must be between 10 and 80");

            // At least one contact method required (from requirements)
            if (!reader.HasValidContact())
                throw new ArgumentException("Either email or phone number must be provided");

            if (!string.IsNullOrWhiteSpace(reader.Email) && !IsValidEmail(reader.Email))
                throw new ArgumentException("Invalid email format");

            if (reader.NumberOfExtensions < 0)
                throw new ArgumentException("Number of extensions cannot be negative");
        }

        /// <summary>
        /// Validates that contact information is unique
        /// </summary>
        /// <param name="reader">Reader to validate</param>
        /// <param name="excludeReaderId">Reader ID to exclude from check (for updates)</param>
        private void ValidateUniqueContact(Reader reader, int? excludeReaderId = null)
        {
            if (!string.IsNullOrWhiteSpace(reader.Email))
            {
                var existingReader = _readerRepository.GetByEmail(reader.Email);
                if (existingReader != null && existingReader.ReaderId != excludeReaderId)
                    throw new InvalidOperationException("A reader with this email already exists");
            }

            if (!string.IsNullOrWhiteSpace(reader.PhoneNumber))
            {
                var existingReader = _readerRepository.GetByPhoneNumber(reader.PhoneNumber);
                if (existingReader != null && existingReader.ReaderId != excludeReaderId)
                    throw new InvalidOperationException("A reader with this phone number already exists");
            }
        }

        /// <summary>
        /// Simple email validation
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if valid</returns>
        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".") && email.Length > 5;
        }
    }
}
