using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for reader data access operations
    /// </summary>
    public interface IReaderDataServices
    {
        /// <summary>
        /// Adds a new reader to the database
        /// </summary>
        /// <param name="reader">The reader</param>
        void AddReader(Reader reader);

        /// <summary>
        /// Retrieves a reader from the database by id
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <returns>A Reader type object containing the info from the database</returns>
        Reader GetReaderById(int readerId);

        /// <summary>
        /// Retrieves a reader from the database by email
        /// </summary>
        /// <param name="email">The reader's email</param>
        /// <returns>A Reader object containing the info from the database</returns>
        Reader GetReaderByEmail(string email);

        /// <summary>
        /// Retrieves a reader from the database by phone number
        /// </summary>
        /// <param name="phoneNumber">The reader's phone number</param>
        /// <returns>A Reader object containing the info from the database</returns>
        Reader GetReaderByPhone(string phoneNumber);

        /// <summary>
        /// Retrieves all the readers from the database
        /// </summary>
        /// <returns>A list containing all the readers from the database</returns>
        IList<Reader> GetAllReaders();

        /// <summary>
        /// Retrieves all staff members from the database
        /// </summary>
        /// <returns>A list containing all staff members from the database</returns>
        IList<Reader> GetAllStaffMembers();

        /// <summary>
        /// Updates an existing reader in the database
        /// </summary>
        /// <param name="reader">The updated reader</param>
        void UpdateReader(Reader reader);

        /// <summary>
        /// Deletes an existing reader from the database by id
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        void DeleteReaderById(int readerId);
    }
}
