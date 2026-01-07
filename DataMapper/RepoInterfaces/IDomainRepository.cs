using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Repository interface for Domain entity data access operations.
    /// </summary>
    public interface IDomainRepository
    {
        /// <summary>
        /// Adds a new domain to the database.
        /// </summary>
        /// <param name="domain">The domain to add</param>
        void Add(Domain domain);

        /// <summary>
        /// Updates an existing domain in the database.
        /// </summary>
        /// <param name="domain">The domain to update</param>
        void Update(Domain domain);

        /// <summary>
        /// Deletes a domain from the database by its identifier.
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        void Delete(int domainId);

        /// <summary>
        /// Gets a domain by its identifier.
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>The domain if found, null otherwise</returns>
        Domain GetById(int domainId);

        /// <summary>
        /// Gets all domains from the database.
        /// </summary>
        /// <returns>List of all domains</returns>
        IList<Domain> GetAll();

        /// <summary>
        /// Gets all ancestor domains for a given domain (parent, grandparent, etc.).
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of ancestor domains</returns>
        IList<Domain> GetAncestors(int domainId);

        /// <summary>
        /// Gets all descendant domains for a given domain (children, grandchildren, etc.).
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of descendant domains</returns>
        IList<Domain> GetDescendants(int domainId);

        /// <summary>
        /// Gets all root domains (domains without parents).
        /// </summary>
        /// <returns>List of root domains</returns>
        IList<Domain> GetRootDomains();

        /// <summary>
        /// Gets all leaf domains (domains without children).
        /// </summary>
        /// <returns>List of leaf domains</returns>
        IList<Domain> GetLeafDomains();

        /// <summary>
        /// Checks if one domain is an ancestor of another.
        /// </summary>
        /// <param name="potentialAncestorId">The potential ancestor domain ID</param>
        /// <param name="descendantId">The descendant domain ID</param>
        /// <returns>True if ancestor-descendant relationship exists</returns>
        bool IsAncestor(int potentialAncestorId, int descendantId);

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();
    }
}
