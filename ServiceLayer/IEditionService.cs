using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// The IEditionService interface for edition management operations
    /// </summary>
    public interface IEditionService
    {
        /// <summary>
        /// Validate and pass an edition object to the data service for creation
        /// </summary>
        /// <param name="edition">The edition</param>
        void AddEdition(Edition edition);

        /// <summary>
        /// Validate and pass an edition object to the data service for updating
        /// </summary>
        /// <param name="edition">The edition</param>
        void UpdateEdition(Edition edition);

        /// <summary>
        /// Get an edition by its identifier
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>The edition if found</returns>
        Edition GetEditionById(int editionId);

        /// <summary>
        /// Get all editions
        /// </summary>
        /// <returns>List of all editions</returns>
        IList<Edition> GetAllEditions();

        /// <summary>
        /// Delete an edition by its identifier
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        void DeleteEdition(int editionId);

        /// <summary>
        /// Get editions by publisher name
        /// </summary>
        /// <param name="publisherName">The publisher name</param>
        /// <returns>List of editions from the publisher</returns>
        IList<Edition> GetByPublisher(string publisherName);

        /// <summary>
        /// Get editions by year of publishing
        /// </summary>
        /// <param name="year">The year of publishing</param>
        /// <returns>List of editions from the year</returns>
        IList<Edition> GetByYear(int year);
    }
}
