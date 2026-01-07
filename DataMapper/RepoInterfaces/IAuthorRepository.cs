using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Repository interface for Author entity data access operations.
    /// </summary>
    public interface IAuthorRepository
    {
        /// <summary>
        /// Adds a new author to the database.
        /// </summary>
        /// <param name="author">The author to add</param>
        void Add(Author author);

        /// <summary>
        /// Updates an existing author in the database.
        /// </summary>
        /// <param name="author">The author to update</param>
        void Update(Author author);

        /// <summary>
        /// Deletes an author from the database by their identifier.
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        void Delete(int authorId);

        /// <summary>
        /// Gets an author by their identifier.
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>The author if found, null otherwise</returns>
        Author GetById(int authorId);

        /// <summary>
        /// Gets all authors from the database.
        /// </summary>
        /// <returns>List of all authors</returns>
        IList<Author> GetAll();

        /// <summary>
        /// Gets authors by name (first name or last name contains search term).
        /// </summary>
        /// <param name="searchTerm">The search term</param>
        /// <returns>List of matching authors</returns>
        IList<Author> GetByName(string searchTerm);

        /// <summary>
        /// Gets all authors who have written books.
        /// </summary>
        /// <returns>List of authors with books</returns>
        IList<Author> GetAuthorsWithBooks();

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();
    }
}
