using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;

namespace ServiceLayer
{
    /// <summary>
    /// The IReaderService interface for reader management operations
    /// </summary>
    public interface IReaderService
    {
        /// <summary>
        /// Validate and pass a reader object to the data service for creation
        /// </summary>
        /// <param name="reader">The reader</param>
        void AddReader(Reader reader);

        /// <summary>
        /// Validate and pass a reader object to the data service for updating
        /// </summary>
        /// <param name="reader">The reader</param>
        void UpdateReader(Reader reader);

        /// <summary>
        /// Get a reader by their identifier
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The reader if found</returns>
        Reader GetReaderById(int readerId);

        /// <summary>
        /// Get all readers in the library
        /// </summary>
        /// <returns>List of all readers</returns>
        IList<Reader> GetAllReaders();

        /// <summary>
        /// Delete a reader by their identifier
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        void DeleteReader(int readerId);
    }
}
