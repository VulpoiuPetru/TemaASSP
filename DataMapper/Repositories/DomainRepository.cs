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
    /// Repository implementation for Domain entity data access operations.
    /// </summary>
    public class DomainRepository : IDomainRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public DomainRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new domain to the database.
        /// </summary>
        /// <param name="domain">The domain to add</param>
        public void Add(Domain domain)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));

            _context.Domains.Add(domain);
        }

        /// <summary>
        /// Updates an existing domain in the database.
        /// </summary>
        /// <param name="domain">The domain to update</param>
        public void Update(Domain domain)
        {
            if (domain == null)
                throw new ArgumentNullException(nameof(domain));

            _context.Entry(domain).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a domain from the database by its identifier.
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        public void Delete(int domainId)
        {
            var domain = GetById(domainId);
            if (domain != null)
            {
                _context.Domains.Remove(domain);
            }
        }

        /// <summary>
        /// Gets a domain by its identifier.
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>The domain if found, null otherwise</returns>
        public Domain GetById(int domainId)
        {
            return _context.Domains
                .Include(d => d.Parent)
                .Include(d => d.Books)
                .FirstOrDefault(d => d.DomainId == domainId);
        }

        /// <summary>
        /// Gets all domains from the database.
        /// </summary>
        /// <returns>List of all domains</returns>
        public IList<Domain> GetAll()
        {
            return _context.Domains
                .Include(d => d.Parent)
                .ToList();
        }

        /// <summary>
        /// Gets all ancestor domains for a given domain (parent, grandparent, etc.).
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of ancestor domains</returns>
        public IList<Domain> GetAncestors(int domainId)
        {
            var ancestors = new List<Domain>();
            var domain = GetById(domainId);

            if (domain == null)
                return ancestors;

            var current = domain.Parent;
            while (current != null)
            {
                ancestors.Add(current);
                current = _context.Domains
                    .Include(d => d.Parent)
                    .FirstOrDefault(d => d.DomainId == current.DomainId)?.Parent;
            }

            return ancestors;
        }

        /// <summary>
        /// Gets all descendant domains for a given domain (children, grandchildren, etc.).
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of descendant domains</returns>
        public IList<Domain> GetDescendants(int domainId)
        {
            var descendants = new List<Domain>();
            var domain = GetById(domainId);

            if (domain == null)
                return descendants;

            GetDescendantsRecursive(domainId, descendants);
            return descendants;
        }

        /// <summary>
        /// Recursively gets all descendants of a domain.
        /// </summary>
        /// <param name="parentId">The parent domain identifier</param>
        /// <param name="descendants">List to accumulate descendants</param>
        private void GetDescendantsRecursive(int parentId, List<Domain> descendants)
        {
            var children = _context.Domains
                .Include(d => d.Parent)
                .Where(d => d.Parent != null && d.Parent.DomainId == parentId)
                .ToList();

            foreach (var child in children)
            {
                descendants.Add(child);
                GetDescendantsRecursive(child.DomainId, descendants);
            }
        }

        /// <summary>
        /// Gets all root domains (domains without parents).
        /// </summary>
        /// <returns>List of root domains</returns>
        public IList<Domain> GetRootDomains()
        {
            return _context.Domains
                .Where(d => d.Parent == null)
                .ToList();
        }

        /// <summary>
        /// Gets all leaf domains (domains without children).
        /// </summary>
        /// <returns>List of leaf domains</returns>
        public IList<Domain> GetLeafDomains()
        {
            var allDomainIds = _context.Domains.Select(d => d.DomainId).ToList();
            var parentIds = _context.Domains
                .Where(d => d.Parent != null)
                .Select(d => d.Parent.DomainId)
                .Distinct()
                .ToList();

            var leafDomainIds = allDomainIds.Except(parentIds).ToList();

            return _context.Domains
                .Where(d => leafDomainIds.Contains(d.DomainId))
                .ToList();
        }

        /// <summary>
        /// Checks if one domain is an ancestor of another.
        /// </summary>
        /// <param name="potentialAncestorId">The potential ancestor domain ID</param>
        /// <param name="descendantId">The descendant domain ID</param>
        /// <returns>True if ancestor-descendant relationship exists</returns>
        public bool IsAncestor(int potentialAncestorId, int descendantId)
        {
            if (potentialAncestorId == descendantId)
                return false;

            var ancestors = GetAncestors(descendantId);
            return ancestors.Any(a => a.DomainId == potentialAncestorId);
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
