using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Interface for Copy repository operations.
    /// </summary>
    public interface ICopyRepository
    {
        /// <summary>
        /// Adds a new copy to the database.
        /// </summary>
        /// <param name="copy">The copy to add</param>
        void Add(Copy copy);

        /// <summary>
        /// Updates an existing copy in the database.
        /// </summary>
        /// <param name="copy">The copy to update</param>
        void Update(Copy copy);

        /// <summary>
        /// Deletes a copy from the database.
        /// </summary>
        /// <param name="id">The copy identifier</param>
        void Delete(int id);

        /// <summary>
        /// Gets a copy by its identifier.
        /// </summary>
        /// <param name="id">The copy identifier</param>
        /// <returns>The copy if found, null otherwise</returns>
        Copy GetById(int id);

        /// <summary>
        /// Gets all copies from the database.
        /// </summary>
        /// <returns>List of all copies</returns>
        IList<Copy> GetAll();

        /// <summary>
        /// Gets all copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of copies for the edition</returns>
        IList<Copy> GetByEditionId(int editionId);

        /// <summary>
        /// Gets all available copies for a specific edition that can be borrowed.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of available borrowable copies</returns>
        IList<Copy> GetAvailableBorrowableCopies(int editionId);

        /// <summary>
        /// Gets the count of available borrowable copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>Count of available borrowable copies</returns>
        int GetAvailableBorrowableCopiesCount(int editionId);

        /// <summary>
        /// Gets the count of reading room only copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>Count of reading room only copies</returns>
        int GetReadingRoomOnlyCopiesCount(int editionId);

        /// <summary>
        /// Gets the total count of copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>Total count of copies</returns>
        int GetTotalCopiesCount(int editionId);
    }
}
