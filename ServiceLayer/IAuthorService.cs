using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// The IAuthorService interface for author management operations
    /// </summary>
    public interface IAuthorService
    {
        /// <summary>
        /// Validate and pass an author object to the data service for creation
        /// </summary>
        /// <param name="author">The author</param>
        void AddAuthor(Author author);

        /// <summary>
        /// Validate and pass an author object to the data service for updating
        /// </summary>
        /// <param name="author">The author</param>
        void UpdateAuthor(Author author);

        /// <summary>
        /// Get an author by their identifier
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>The author if found</returns>
        Author GetAuthorById(int authorId);

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of all authors</returns>
        IList<Author> GetAllAuthors();

        /// <summary>
        /// Delete an author by their identifier
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        void DeleteAuthor(int authorId);
    }
}
