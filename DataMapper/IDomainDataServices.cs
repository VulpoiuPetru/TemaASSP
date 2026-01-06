using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for domain data access operations
    /// </summary>
    public interface IDomainDataServices
    {
        /// <summary>
        /// Adds a new domain to the database
        /// </summary>
        /// <param name="domain">The domain</param>
        void AddDomain(Domain domain);

        /// <summary>
        /// Retrieves a domain from the database by id
        /// </summary>
        /// <param name="domainId">The domain's id</param>
        /// <returns>A Domain object containing the info from the database</returns>
        Domain GetDomainById(int domainId);

        /// <summary>
        /// Retrieves multiple domains from the database by their ids
        /// </summary>
        /// <param name="domainIds">The list of domain ids</param>
        /// <returns>A list containing the domains from the database</returns>
        IList<Domain> GetDomainsByIds(IList<int> domainIds);

        /// <summary>
        /// Retrieves all domains from the database
        /// </summary>
        /// <returns>A list containing all domains from the database</returns>
        IList<Domain> GetAllDomains();

        /// <summary>
        /// Retrieves all top-level domains (no parent) from the database
        /// </summary>
        /// <returns>A list containing all root domains</returns>
        IList<Domain> GetAllRootDomains();

        /// <summary>
        /// Retrieves all child domains of a specific parent domain
        /// </summary>
        /// <param name="parentDomainId">The parent domain's id</param>
        /// <returns>A list containing all child domains</returns>
        IList<Domain> GetChildDomainsByParent(int parentDomainId);

        /// <summary>
        /// Updates an existing domain in the database
        /// </summary>
        /// <param name="domain">The updated domain</param>
        void UpdateDomain(Domain domain);

        /// <summary>
        /// Deletes an existing domain from the database by id
        /// </summary>
        /// <param name="domainId">The domain's id</param>
        void DeleteDomainById(int domainId);
    }
}
