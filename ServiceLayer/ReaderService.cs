using DataMapper;
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
        // TODO: Add repository when Data Layer is implemented
        private static List<Reader> _readers = new List<Reader>(); // Temporary in-memory storage

        /// <summary>
        /// Initializes a new instance of the ReaderService class
        /// </summary>
        public ReaderService()
        {
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

            // Assign new ID (temporary - will be handled by database)
            reader.ReaderId = _readers.Count > 0 ? _readers.Max(r => r.ReaderId) + 1 : 1;

            // Add to storage
            _readers.Add(reader);
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

            // Update the existing reader
            var index = _readers.FindIndex(r => r.ReaderId == reader.ReaderId);
            _readers[index] = reader;
        }

        /// <summary>
        /// Get a reader by their identifier
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The reader if found</returns>
        public Reader GetReaderById(int readerId)
        {
            return _readers.FirstOrDefault(r => r.ReaderId == readerId);
        }

        /// <summary>
        /// Get all readers in the library
        /// </summary>
        /// <returns>List of all readers</returns>
        public IList<Reader> GetAllReaders()
        {
            return _readers.ToList();
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

            _readers.RemoveAll(r => r.ReaderId == readerId);
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
            var existingReaders = _readers.Where(r => excludeReaderId == null || r.ReaderId != excludeReaderId);

            if (!string.IsNullOrWhiteSpace(reader.Email))
            {
                if (existingReaders.Any(r => r.Email?.Equals(reader.Email, StringComparison.OrdinalIgnoreCase) == true))
                    throw new InvalidOperationException("A reader with this email already exists");
            }

            if (!string.IsNullOrWhiteSpace(reader.PhoneNumber))
            {
                if (existingReaders.Any(r => r.PhoneNumber?.Equals(reader.PhoneNumber) == true))
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
            return email.Contains("@") && email.Contains(". ") && email.Length > 5;
        }
    }
}
