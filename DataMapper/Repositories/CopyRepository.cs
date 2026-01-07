using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.Repositories
{
    /// <summary>
    /// Repository implementation for Copy entity using Data Mapper pattern.
    /// </summary>
    public class CopyRepository : ICopyRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public CopyRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new copy to the database.
        /// </summary>
        /// <param name="copy">The copy to add</param>
        public void Add(Copy copy)
        {
            if (copy == null)
                throw new ArgumentNullException(nameof(copy));

            _context.Copies.Add(copy);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing copy in the database.
        /// </summary>
        /// <param name="copy">The copy to update</param>
        public void Update(Copy copy)
        {
            if (copy == null)
                throw new ArgumentNullException(nameof(copy));

            var existingCopy = _context.Copies.Find(copy.Id);
            if (existingCopy == null)
                throw new InvalidOperationException($"Copy with ID {copy.Id} not found");

            _context.Entry(existingCopy).CurrentValues.SetValues(copy);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes a copy from the database.
        /// </summary>
        /// <param name="id">The copy identifier</param>
        public void Delete(int id)
        {
            var copy = _context.Copies.Find(id);
            if (copy == null)
                throw new InvalidOperationException($"Copy with ID {id} not found");

            _context.Copies.Remove(copy);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a copy by its identifier.
        /// </summary>
        /// <param name="id">The copy identifier</param>
        /// <returns>The copy if found, null otherwise</returns>
        public Copy GetById(int id)
        {
            return _context.Copies
                .Include(c => c.Edition)
                .FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Gets all copies from the database.
        /// </summary>
        /// <returns>List of all copies</returns>
        public IList<Copy> GetAll()
        {
            return _context.Copies
                .Include(c => c.Edition)
                .ToList();
        }

        /// <summary>
        /// Gets all copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of copies for the edition</returns>
        public IList<Copy> GetByEditionId(int editionId)
        {
            return _context.Copies
                .Include(c => c.Edition)
                .Where(c => c.Edition.EditionId == editionId)
                .ToList();
        }

        /// <summary>
        /// Gets all available copies for a specific edition that can be borrowed.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of available borrowable copies</returns>
        public IList<Copy> GetAvailableBorrowableCopies(int editionId)
        {
            return _context.Copies
                .Include(c => c.Edition)
                .Where(c => c.Edition.EditionId == editionId
                    && !c.IsReadingRoomOnly
                    && c.IsAvailable)
                .ToList();
        }

        /// <summary>
        /// Gets the count of available borrowable copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>Count of available borrowable copies</returns>
        public int GetAvailableBorrowableCopiesCount(int editionId)
        {
            return _context.Copies
                .Count(c => c.Edition.EditionId == editionId
                    && !c.IsReadingRoomOnly
                    && c.IsAvailable);
        }

        /// <summary>
        /// Gets the count of reading room only copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>Count of reading room only copies</returns>
        public int GetReadingRoomOnlyCopiesCount(int editionId)
        {
            return _context.Copies
                .Count(c => c.Edition.EditionId == editionId
                    && c.IsReadingRoomOnly);
        }

        /// <summary>
        /// Gets the total count of copies for a specific edition.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>Total count of copies</returns>
        public int GetTotalCopiesCount(int editionId)
        {
            return _context.Copies
                .Count(c => c.Edition.EditionId == editionId);
        }
    }
}
