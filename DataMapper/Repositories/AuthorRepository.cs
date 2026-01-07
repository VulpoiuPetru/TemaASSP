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
    /// Repository implementation for Author entity data access operations.
    /// </summary>
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public AuthorRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new author to the database.
        /// </summary>
        /// <param name="author">The author to add</param>
        public void Add(Author author)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author));

            _context.Authors.Add(author);
        }

        /// <summary>
        /// Updates an existing author in the database.
        /// </summary>
        /// <param name="author">The author to update</param>
        public void Update(Author author)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author));

            _context.Entry(author).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes an author from the database by their identifier.
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        public void Delete(int authorId)
        {
            var author = GetById(authorId);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }
        }

        /// <summary>
        /// Gets an author by their identifier.
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>The author if found, null otherwise</returns>
        public Author GetById(int authorId)
        {
            return _context.Authors
                .Include(a => a.Books)
                .FirstOrDefault(a => a.AuthorId == authorId);
        }

        /// <summary>
        /// Gets all authors from the database.
        /// </summary>
        /// <returns>List of all authors</returns>
        public IList<Author> GetAll()
        {
            return _context.Authors
                .Include(a => a.Books)
                .ToList();
        }

        /// <summary>
        /// Gets authors by name (first name or last name contains search term).
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns>List of matching authors</returns>
        public IList<Author> GetByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Author>();

            return _context.Authors
                .Include(a => a.Books)
                .Where(a => a.FirstName.Contains(searchTerm) || a.LastName.Contains(searchTerm))
                .ToList();
        }

        /// <summary>
        /// Gets all authors who have written books.
        /// </summary>
        /// <returns>List of authors with books</returns>
        public IList<Author> GetAuthorsWithBooks()
        {
            return _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Books.Any())
                .ToList();
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
