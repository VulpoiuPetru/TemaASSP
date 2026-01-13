using DataMapper.RepoInterfaces;
using DomainModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for domain management operations with business logic validation
    /// </summary>
    public class DomainService : IDomainService
    {
        private readonly IDomainRepository _domainRepository;
        private readonly ILogger<DomainService> _logger;
        private readonly IValidator<Domain> _validator;

        /// <summary>
        /// Initializes a new instance of the DomainService class
        /// </summary>
        /// <param name="domainRepository">Domain repository</param>
        /// <param name="logger">Logger instance</param>
        /// <param name="validator">FluentValidation validator for Domain</param>
        public DomainService(IDomainRepository domainRepository, ILogger<DomainService> logger, IValidator<Domain> validator)
        {
            _domainRepository = domainRepository ?? throw new ArgumentNullException(nameof(domainRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Validate and pass a domain object to the data service for creation
        /// </summary>
        /// <param name="domain">The domain</param>
        public void AddDomain(Domain domain)
        {
            try
            {
                if (domain == null)
                    throw new ArgumentNullException(nameof(domain), "Domain cannot be null");

                ValidateDomain(domain);

                if (domain.Parent != null)
                {
                    var parentDomain = GetDomainById(domain.Parent.DomainId);
                    if (parentDomain == null)
                        throw new InvalidOperationException($"Parent domain with ID {domain.Parent.DomainId} not found");
                }

                _domainRepository.Add(domain);
                _domainRepository.SaveChanges();

                _logger.LogInformation("Domain added successfully: {Name}", domain.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding domain: {Name}", domain?.Name);
                throw;
            }
        }

        /// <summary>
        /// Validate and pass a domain object to the data service for updating
        /// </summary>
        /// <param name="domain">The domain</param>
        public void UpdateDomain(Domain domain)
        {
            try
            {
                if (domain == null)
                    throw new ArgumentNullException(nameof(domain), "Domain cannot be null");

                var existingDomain = GetDomainById(domain.DomainId);
                if (existingDomain == null)
                    throw new InvalidOperationException($"Domain with ID {domain.DomainId} not found");

                ValidateDomain(domain);

                if (domain.Parent != null && domain.Parent.DomainId == domain.DomainId)
                    throw new InvalidOperationException("Domain cannot be its own parent");

                if (domain.Parent != null)
                {
                    if (_domainRepository.IsAncestor(domain.DomainId, domain.Parent.DomainId))
                        throw new InvalidOperationException("Cannot set parent to a descendant domain (circular reference)");
                }

                _domainRepository.Update(domain);
                _domainRepository.SaveChanges();

                _logger.LogInformation("Domain updated successfully: ID {DomainId}", domain.DomainId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating domain with ID: {DomainId}", domain?.DomainId);
                throw;
            }
        }

        /// <summary>
        /// Get a domain by its identifier
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>The domain if found</returns>
        public Domain GetDomainById(int domainId)
        {
            try
            {
                return _domainRepository.GetById(domainId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving domain with ID: {DomainId}", domainId);
                throw;
            }
        }

        /// <summary>
        /// Get all domains
        /// </summary>
        /// <returns>List of all domains</returns>
        public IList<Domain> GetAllDomains()
        {
            try
            {
                return _domainRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all domains");
                throw;
            }
        }

        /// <summary>
        /// Delete a domain by its identifier
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        public void DeleteDomain(int domainId)
        {
            try
            {
                var domain = GetDomainById(domainId);
                if (domain == null)
                    throw new InvalidOperationException($"Domain with ID {domainId} not found");

                if (domain.Books?.Any() == true)
                    throw new InvalidOperationException("Cannot delete domain with associated books");

                var descendants = _domainRepository.GetDescendants(domainId);
                if (descendants.Any())
                    throw new InvalidOperationException("Cannot delete domain with child domains");

                _domainRepository.Delete(domainId);
                _domainRepository.SaveChanges();

                _logger.LogInformation("Domain deleted successfully: ID {DomainId}", domainId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting domain with ID: {DomainId}", domainId);
                throw;
            }
        }

        /// <summary>
        /// Get all root domains (domains without parents)
        /// </summary>
        /// <returns>List of root domains</returns>
        public IList<Domain> GetRootDomains()
        {
            try
            {
                return _domainRepository.GetRootDomains();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all copies");
                throw;
            }
        }

        /// <summary>
        /// Get all leaf domains (domains without children)
        /// </summary>
        /// <returns>List of leaf domains</returns>
        public IList<Domain> GetLeafDomains()
        {
            return _domainRepository.GetLeafDomains();
        }

        /// <summary>
        /// Get all ancestor domains for a given domain
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of ancestor domains</returns>
        public IList<Domain> GetAncestors(int domainId)
        {
            return _domainRepository.GetAncestors(domainId);
        }

        /// <summary>
        /// Get all descendant domains for a given domain
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of descendant domains</returns>
        public IList<Domain> GetDescendants(int domainId)
        {
            return _domainRepository.GetDescendants(domainId);
        }

        /// <summary>
        /// Validates domain data according to business rules
        /// </summary>
        /// <param name="domain">Domain to validate</param>
        private void ValidateDomain(Domain domain)
        {
            var result = _validator.Validate(domain);

            if (!result.IsValid)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Domain validation failed: {errors}");
            }
        }

    }
}
