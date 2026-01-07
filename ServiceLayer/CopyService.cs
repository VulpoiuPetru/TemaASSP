using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for copy management operations with business logic validation
    /// </summary>
    public class CopyService : ICopyService
    {
        private readonly ICopyRepository _copyRepository;
        private readonly IEditionRepository _editionRepository;

        /// <summary>
        /// Initializes a new instance of the CopyService class
        /// </summary>
        /// <param name="copyRepository">Copy repository</param>
        /// <param name="editionRepository">Edition repository</param>
        public CopyService(ICopyRepository copyRepository, IEditionRepository editionRepository)
        {
            _copyRepository = copyRepository ?? throw new ArgumentNullException(nameof(copyRepository));
            _editionRepository = editionRepository ?? throw new ArgumentNullException(nameof(editionRepository));
        }

        /// <summary>
        /// Validate and pass a copy object to the data service for creation
        /// </summary>
        /// <param name="copy">The copy</param>
        public void AddCopy(Copy copy)
        {
            if (copy == null)
                throw new ArgumentNullException(nameof(copy), "Copy cannot be null");

            ValidateCopy(copy);

            if (copy.Edition != null)
            {
                var edition = _editionRepository.GetById(copy.Edition.EditionId);
                if (edition == null)
                    throw new InvalidOperationException($"Edition with ID {copy.Edition.EditionId} not found");
            }

            _copyRepository.Add(copy);
        }

        /// <summary>
        /// Validate and pass a copy object to the data service for updating
        /// </summary>
        /// <param name="copy">The copy</param>
        public void UpdateCopy(Copy copy)
        {
            if (copy == null)
                throw new ArgumentNullException(nameof(copy), "Copy cannot be null");

            var existingCopy = GetCopyById(copy.Id);
            if (existingCopy == null)
                throw new InvalidOperationException($"Copy with ID {copy.Id} not found");

            ValidateCopy(copy);

            _copyRepository.Update(copy);
        }

        /// <summary>
        /// Get a copy by its identifier
        /// </summary>
        /// <param name="id">The copy identifier</param>
        /// <returns>The copy if found</returns>
        public Copy GetCopyById(int id)
        {
            return _copyRepository.GetById(id);
        }

        /// <summary>
        /// Get all copies
        /// </summary>
        /// <returns>List of all copies</returns>
        public IList<Copy> GetAllCopies()
        {
            return _copyRepository.GetAll();
        }

        /// <summary>
        /// Delete a copy by its identifier
        /// </summary>
        /// <param name="id">The copy identifier</param>
        public void DeleteCopy(int id)
        {
            var copy = GetCopyById(id);
            if (copy == null)
                throw new InvalidOperationException($"Copy with ID {id} not found");

            if (!copy.IsAvailable)
                throw new InvalidOperationException("Cannot delete copy that is currently borrowed");

            _copyRepository.Delete(id);
        }

        /// <summary>
        /// Get all copies for a specific edition
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of copies for the edition</returns>
        public IList<Copy> GetByEditionId(int editionId)
        {
            return _copyRepository.GetByEditionId(editionId);
        }

        /// <summary>
        /// Get all available borrowable copies for a specific edition
        /// </summary>
        /// <param name="editionId">The edition identifier</param>
        /// <returns>List of available borrowable copies</returns>
        public IList<Copy> GetAvailableBorrowableCopies(int editionId)
        {
            return _copyRepository.GetAvailableBorrowableCopies(editionId);
        }

        /// <summary>
        /// Mark a copy as borrowed
        /// </summary>
        /// <param name="copyId">The copy identifier</param>
        public void MarkAsBorrowed(int copyId)
        {
            var copy = GetCopyById(copyId);
            if (copy == null)
                throw new InvalidOperationException($"Copy with ID {copyId} not found");

            if (!copy.IsAvailable)
                throw new InvalidOperationException("Copy is already borrowed");

            if (copy.IsReadingRoomOnly)
                throw new InvalidOperationException("Reading room only copies cannot be borrowed");

            copy.IsAvailable = false;
            UpdateCopy(copy);
        }

        /// <summary>
        /// Mark a copy as returned
        /// </summary>
        /// <param name="copyId">The copy identifier</param>
        public void MarkAsReturned(int copyId)
        {
            var copy = GetCopyById(copyId);
            if (copy == null)
                throw new InvalidOperationException($"Copy with ID {copyId} not found");

            if (copy.IsAvailable)
                throw new InvalidOperationException("Copy is already available");

            copy.IsAvailable = true;
            UpdateCopy(copy);
        }

        /// <summary>
        /// Validates copy data according to business rules
        /// </summary>
        /// <param name="copy">Copy to validate</param>
        private void ValidateCopy(Copy copy)
        {
            if (copy.Edition == null)
                throw new ArgumentException("Copy must be associated with an edition");
        }
    }
}
