using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// The ICopyService interface for copy management operations
    /// </summary>
    public interface ICopyService
    {
        /// <summary>
        /// Validate and pass a copy object to the data service for creation
        /// </summary>
        /// <param name="copy">The copy</param>
        void AddCopy(Copy copy);

        /// <summary>
        /// Validate and pass a copy object to the data service for updating
        /// </summary>
        /// <param name="copy">The copy</param>
        void UpdateCopy(Copy copy);

        /// <summary>
        /// Get a copy by its identifier
        /// </summary>
        /// <param name="id">The copy identifier</param>
        /// <returns>The copy if found</returns>
        Copy GetCopyById(int id);

        /// <summary>
        /// Get all copies
        /// </summary>
        /// <returns>List of all copies</returns>
        IList<Copy> GetAllCopies();

        /// <summary>
        /// Delete a copy by its identifier
        /// </summary>
        /// <param name="id">The copy identifier</param>
        void DeleteCopy(int id);

        /// <summary>
        /// Get all copies for a specific edition
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of copies for the edition</returns>
        IList<Copy> GetByEditionId(int editionId);

        /// <summary>
        /// Get all available borrowable copies for a specific edition
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of available borrowable copies</returns>
        IList<Copy> GetAvailableBorrowableCopies(int editionId);

        /// <summary>
        /// Mark a copy as borrowed
        /// </summary>
        /// <param name="copyId">The copy identifier</param>
        void MarkAsBorrowed(int copyId);

        /// <summary>
        /// Mark a copy as returned
        /// </summary>
        /// <param name="copyId">The copy identifier</param>
        void MarkAsReturned(int copyId);
    }
}
