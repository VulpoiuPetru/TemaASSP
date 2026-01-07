using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.Repositories
{
    /// <summary>
    /// Repository implementation for Reader entity data access operations.
    /// </summary>
    public class ReaderRepository : IReaderRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public ReaderRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new reader to the database.
        /// </summary>
        /// <param name="reader">The reader to add</param>
        public void Add(Reader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _context.Readers.Add(reader);
        }

        /// <summary>
        /// Updates an existing reader in the database.
        /// </summary>
        /// <param name="reader">The reader to update</param>
        public void Update(Reader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            _context.Entry(reader).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a reader from the database by their identifier.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        public void Delete(int readerId)
        {
            var reader = GetById(readerId);
            if (reader != null)
            {
                _context.Readers.Remove(reader);
            }
        }

        /// <summary>
        /// Gets a reader by their identifier.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The reader if found, null otherwise</returns>
        public Reader GetById(int readerId)
        {
            return _context.Readers
                .Include(r => r.BorrowedBooks)
                .Include(r => r.Extensions)
                .FirstOrDefault(r => r.ReaderId == readerId);
        }

        /// <summary>
        /// Gets all readers from the database.
        /// </summary>
        /// <returns>List of all readers</returns>
        public IList<Reader> GetAll()
        {
            return _context.Readers
                .Include(r => r.BorrowedBooks)
                .Include(r => r.Extensions)
                .ToList();
        }

        /// <summary>
        /// Gets a reader by email address.
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>The reader if found, null otherwise</returns>
        public Reader GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return _context.Readers
                .FirstOrDefault(r => r.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a reader by phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number</param>
        /// <returns>The reader if found, null otherwise</returns>
        public Reader GetByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            return _context.Readers
                .FirstOrDefault(r => r.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// Gets all employee readers.
        /// </summary>
        /// <returns>List of employee readers</returns>
        public IList<Reader> GetEmployees()
        {
            return _context.Readers
                .Where(r => r.IsEmployee)
                .ToList();
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
