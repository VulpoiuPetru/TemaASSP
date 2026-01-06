using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for edition data access operations
    /// </summary>
    public interface IEditionDataServices
    {
        /// <summary>
        /// Adds a new edition to the database
        /// </summary>
        /// <param name="edition">The edition</param>
        void AddEdition(Edition edition);

        /// <summary>
        /// Retrieves an edition from the database by id
        /// </summary>
        /// <param name="editionId">The edition's id</param>
        /// <returns>An Edition object containing the info from the database</returns>
        Edition GetEditionById(int editionId);

        /// <summary>
        /// Retrieves the edition for a specific book from the database
        /// </summary>
        /// <param name="bookId">The book's id</param>
        /// <returns>An Edition object containing the info from the database</returns>
        Edition GetEditionByBookId(int bookId);

        /// <summary>
        /// Retrieves all editions from the database
        /// </summary>
        /// <returns>A list containing all editions from the database</returns>
        IList<Edition> GetAllEditions();

        /// <summary>
        /// Retrieves editions by publisher from the database
        /// </summary>
        /// <param name="publisher">The publisher name</param>
        /// <returns>A list containing editions by the specified publisher</returns>
        IList<Edition> GetEditionsByPublisher(string publisher);

        /// <summary>
        /// Retrieves editions by publication year from the database
        /// </summary>
        /// <param name="year">The publication year</param>
        /// <returns>A list containing editions published in the specified year</returns>
        IList<Edition> GetEditionsByYear(int year);

        /// <summary>
        /// Retrieves editions by type (paperback, hardcover) from the database
        /// </summary>
        /// <param name="type">The edition type</param>
        /// <returns>A list containing editions of the specified type</returns>
        IList<Edition> GetEditionsByType(string type);

        /// <summary>
        /// Updates an existing edition in the database
        /// </summary>
        /// <param name="edition">The updated edition</param>
        void UpdateEdition(Edition edition);

        /// <summary>
        /// Deletes an existing edition from the database by id
        /// </summary>
        /// <param name="editionId">The edition's id</param>
        void DeleteEditionById(int editionId);
    }
}
