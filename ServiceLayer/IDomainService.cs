using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// The IDomainService interface for domain management operations
    /// </summary>
    public interface IDomainService
    {
        /// <summary>
        /// Validate and pass a domain object to the data service for creation
        /// </summary>
        /// <param name="domain">The domain</param>
        void AddDomain(Domain domain);

        /// <summary>
        /// Validate and pass a domain object to the data service for updating
        /// </summary>
        /// <param name="domain">The domain</param>
        void UpdateDomain(Domain domain);

        /// <summary>
        /// Get a domain by its identifier
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>The domain if found</returns>
        Domain GetDomainById(int domainId);

        /// <summary>
        /// Get all domains
        /// </summary>
        /// <returns>List of all domains</returns>
        IList<Domain> GetAllDomains();

        /// <summary>
        /// Delete a domain by its identifier
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        void DeleteDomain(int domainId);

        /// <summary>
        /// Get all root domains (domains without parents)
        /// </summary>
        /// <returns>List of root domains</returns>
        IList<Domain> GetRootDomains();

        /// <summary>
        /// Get all leaf domains (domains without children)
        /// </summary>
        /// <returns>List of leaf domains</returns>
        IList<Domain> GetLeafDomains();

        /// <summary>
        /// Get all ancestor domains for a given domain
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of ancestor domains</returns>
        IList<Domain> GetAncestors(int domainId);

        /// <summary>
        /// Get all descendant domains for a given domain
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of descendant domains</returns>
        IList<Domain> GetDescendants(int domainId);
    }
}
