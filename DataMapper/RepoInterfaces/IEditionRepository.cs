using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Repository interface for Edition entity data access operations.
    /// </summary>
    public interface IEditionRepository
    {
        /// <summary>
        /// Adds a new edition to the database.
        /// </summary>
        /// <param name="edition">The edition to add</param>
        void Add(Edition edition);

        /// <summary>
        /// Updates an existing edition in the database.
        /// </summary>
        /// <param name="edition">The edition to update</param>
        void Update(Edition edition);

        /// <summary>
        /// Deletes an edition from the database by its identifier.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        void Delete(int editionId);

        /// <summary>
        /// Gets an edition by its identifier.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>The edition if found, null otherwise</returns>
        Edition GetById(int editionId);

        /// <summary>
        /// Gets all editions from the database.
        /// </summary>
        /// <returns>List of all editions</returns>
        IList<Edition> GetAll();

        /// <summary>
        /// Gets editions by publisher name.
        /// </summary>
        /// <param name="publisherName">The publisher name</param>
        /// <returns>List of editions from the specified publisher</returns>
        IList<Edition> GetByPublisher(string publisherName);

        /// <summary>
        /// Gets editions by year of publishing.
        /// </summary>
        /// <param name="year">The year of publishing</param>
        /// <returns>List of editions from the specified year</returns>
        IList<Edition> GetByYear(int year);

        /// <summary>
        /// Gets the edition for a specific book.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>The edition if found, null otherwise</returns>
        Edition GetByBookId(int bookId);

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();
    }
}
