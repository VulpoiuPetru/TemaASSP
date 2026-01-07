using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.Repositories
{
    /// <summary>
    /// Repository implementation for Edition entity data access operations.
    /// </summary>
    public class EditionRepository : IEditionRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public EditionRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new edition to the database.
        /// </summary>
        /// <param name="edition">The edition to add</param>
        public void Add(Edition edition)
        {
            if (edition == null)
                throw new ArgumentNullException(nameof(edition));

            _context.Editions.Add(edition);
        }

        /// <summary>
        /// Updates an existing edition in the database.
        /// </summary>
        /// <param name="edition">The edition to update</param>
        public void Update(Edition edition)
        {
            if (edition == null)
                throw new ArgumentNullException(nameof(edition));

            _context.Entry(edition).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes an edition from the database by its identifier.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        public void Delete(int editionId)
        {
            var edition = GetById(editionId);
            if (edition != null)
            {
                _context.Editions.Remove(edition);
            }
        }

        /// <summary>
        /// Gets an edition by its identifier.
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>The edition if found, null otherwise</returns>
        public Edition GetById(int editionId)
        {
            return _context.Editions
                .Include(e => e.Book)
                .Include(e => e.Copies)
                .FirstOrDefault(e => e.EditionId == editionId);
        }

        /// <summary>
        /// Gets all editions from the database.
        /// </summary>
        /// <returns>List of all editions</returns>
        public IList<Edition> GetAll()
        {
            return _context.Editions
                .Include(e => e.Book)
                .Include(e => e.Copies)
                .ToList();
        }

        /// <summary>
        /// Gets editions by publisher name.
        /// </summary>
        /// <param name="publisherName">The publisher name</param>
        /// <returns>List of editions from the specified publisher</returns>
        public IList<Edition> GetByPublisher(string publisherName)
        {
            if (string.IsNullOrWhiteSpace(publisherName))
                return new List<Edition>();

            return _context.Editions
                .Include(e => e.Book)
                .Where(e => e.Publisher.Contains(publisherName))
                .ToList();
        }

        /// <summary>
        /// Gets editions by year of publishing.
        /// </summary>
        /// <param name="year">The year of publishing</param>
        /// <returns>List of editions from the specified year</returns>
        public IList<Edition> GetByYear(int year)
        {
            return _context.Editions
                .Include(e => e.Book)
                .Where(e => e.YearOfPublishing == year)
                .ToList();
        }

        /// <summary>
        /// Gets the edition for a specific book.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>The edition if found, null otherwise</returns>
        public Edition GetByBookId(int bookId)
        {
            return _context.Editions
                .Include(e => e.Book)
                .Include(e => e.Copies)
                .FirstOrDefault(e => e.EditionId == bookId);
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
