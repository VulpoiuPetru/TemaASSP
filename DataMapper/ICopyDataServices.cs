using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for copy data access operations
    /// </summary>
    public interface ICopyDataServices
    {
        /// <summary>
        /// Adds a new copy to the database
        /// </summary>
        /// <param name="copy">The copy</param>
        void AddCopy(Copy copy);

        /// <summary>
        /// Retrieves a copy from the database by id
        /// </summary>
        /// <param name="copyId">The copy's id</param>
        /// <returns>A Copy object containing the info from the database</returns>
        Copy GetCopyById(int copyId);

        /// <summary>
        /// Retrieves all copies for a specific edition from the database
        /// </summary>
        /// <param name="editionId">The edition's id</param>
        /// <returns>A list containing all copies for the edition</returns>
        IList<Copy> GetCopiesByEdition(int editionId);

        /// <summary>
        /// Retrieves all available copies for borrowing from the database
        /// </summary>
        /// <returns>A list containing all available copies for borrowing</returns>
        IList<Copy> GetAllAvailableCopies();

        /// <summary>
        /// Retrieves all reading room only copies from the database
        /// </summary>
        /// <returns>A list containing all reading room only copies</returns>
        IList<Copy> GetAllReadingRoomCopies();

        /// <summary>
        /// Retrieves available copies for a specific edition
        /// </summary>
        /// <param name="editionId">The edition's id</param>
        /// <returns>A list containing available copies for the edition</returns>
        IList<Copy> GetAvailableCopiesByEdition(int editionId);

        /// <summary>
        /// Retrieves all copies from the database
        /// </summary>
        /// <returns>A list containing all copies from the database</returns>
        IList<Copy> GetAllCopies();

        /// <summary>
        /// Updates an existing copy in the database
        /// </summary>
        /// <param name="copy">The updated copy</param>
        void UpdateCopy(Copy copy);

        /// <summary>
        /// Updates the availability status of a copy
        /// </summary>
        /// <param name="copyId">The copy's id</param>
        /// <param name="isAvailable">The new availability status</param>
        void UpdateCopyAvailability(int copyId, bool isAvailable);

        /// <summary>
        /// Deletes an existing copy from the database by id
        /// </summary>
        /// <param name="copyId">The copy's id</param>
        void DeleteCopyById(int copyId);
    }
}
