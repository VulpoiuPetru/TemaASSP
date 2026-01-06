using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for author data access operations
    /// </summary>
    public interface IAuthorDataServices
    {
        /// <summary>
        /// Adds a new author to the database
        /// </summary>
        /// <param name="author">The author</param>
        void AddAuthor(Author author);

        /// <summary>
        /// Retrieves an author from the database by id
        /// </summary>
        /// <param name="authorId">The author's id</param>
        /// <returns>An Author object containing the info from the database</returns>
        Author GetAuthorById(int authorId);

        /// <summary>
        /// Retrieves authors from the database by name
        /// </summary>
        /// <param name="firstName">The author's first name</param>
        /// <param name="lastName">The author's last name</param>
        /// <returns>An Author object containing the info from the database</returns>
        Author GetAuthorByName(string firstName, string lastName);

        /// <summary>
        /// Searches authors from the database by partial name match
        /// </summary>
        /// <param name="searchTerm">The search term for author name</param>
        /// <returns>A list containing matching authors from the database</returns>
        IList<Author> SearchAuthorsByName(string searchTerm);

        /// <summary>
        /// Retrieves all authors from the database
        /// </summary>
        /// <returns>A list containing all authors from the database</returns>
        IList<Author> GetAllAuthors();

        /// <summary>
        /// Retrieves authors by age range from the database
        /// </summary>
        /// <param name="minAge">The minimum age</param>
        /// <param name="maxAge">The maximum age</param>
        /// <returns>A list containing authors in the specified age range</returns>
        IList<Author> GetAuthorsByAgeRange(int minAge, int maxAge);

        /// <summary>
        /// Updates an existing author in the database
        /// </summary>
        /// <param name="author">The updated author</param>
        void UpdateAuthor(Author author);

        /// <summary>
        /// Deletes an existing author from the database by id
        /// </summary>
        /// <param name="authorId">The author's id</param>
        void DeleteAuthorById(int authorId);
    }
}
