using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Repository interface for Reader entity data access operations.
    /// </summary>
    public interface IReaderRepository
    {
        /// <summary>
        /// Adds a new reader to the database.
        /// </summary>
        /// <param name="reader">The reader to add</param>
        void Add(Reader reader);

        /// <summary>
        /// Updates an existing reader in the database.
        /// </summary>
        /// <param name="reader">The reader to update</param>
        void Update(Reader reader);

        /// <summary>
        /// Deletes a reader from the database by their identifier.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        void Delete(int readerId);

        /// <summary>
        /// Gets a reader by their identifier.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The reader if found, null otherwise</returns>
        Reader GetById(int readerId);

        /// <summary>
        /// Gets all readers from the database.
        /// </summary>
        /// <returns>List of all readers</returns>
        IList<Reader> GetAll();

        /// <summary>
        /// Gets a reader by email address.
        /// </summary>
        /// <param name="email">The email address</param>
        /// <returns>The reader if found, null otherwise</returns>
        Reader GetByEmail(string email);

        /// <summary>
        /// Gets a reader by phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number</param>
        /// <returns>The reader if found, null otherwise</returns>
        Reader GetByPhoneNumber(string phoneNumber);

        /// <summary>
        /// Gets all employee readers.
        /// </summary>
        /// <returns>List of employee readers</returns>
        IList<Reader> GetEmployees();

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();
    }
}
