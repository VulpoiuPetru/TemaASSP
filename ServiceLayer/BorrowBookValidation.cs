using DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for validating book borrowing based on business rules and constraints
    /// </summary>
    public class BorrowBookValidation : IBorrowBookValidation
    {
        private readonly IConfigurationService _configService;

        /// <summary>
        /// Initializes a new instance of the BorrowBookValidation class
        /// </summary>
        public BorrowBookValidation(IConfigurationService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        /// <summary>
        /// Function that validates that a book can be borrowed when 1 or more copies are not marked as being for the lecture hall
        /// </summary>
        public bool ValidateIfBookCanBeBorrowed(Book book)
        {
            // Check if all copies are reading room only
            return book.NumberOfTotalBooks > book.NumberOfReadingRoomBooks;
        }

        /// <summary>
        /// Function that validates that a book can be borrowed if the number of available books is at least 10% of the initial number of available books
        /// </summary>
        public bool ValidateIfThereAreAvailableCopiesToBorrow(Book book)
        {
            var borrowableFund = book.NumberOfTotalBooks - book.NumberOfReadingRoomBooks;
            if (borrowableFund <= 0) return false;

            var minRequired = Math.Ceiling(borrowableFund * 0.1); // 10% rule
            return book.NumberOfAvailableBooks >= minRequired;
        }

        /// <summary>
        /// Function that validates that a reader can borrow a maximum of C books at a time
        /// </summary>
        public bool ValidateNumberOfBorrowedBooks(IList<Book> books, Reader reader)
        {
            var limits = _configService.GetReaderLimits(reader);
            return books.Count <= limits.C;
        }

        /// <summary>
        /// Function that validates that if a reader borrows three or more books then all must belong to at least two different categories
        /// </summary>
        public bool ValidateDomainsForMoreThanThreeBooks(IList<Book> books)
        {
            if (books.Count < 3) return true; // Rule doesn't apply

            // Get all unique domains (including parent domains)
            var allDomains = new HashSet<int>();
            foreach (var book in books)
            {
                foreach (var domain in book.Domains)
                {
                    allDomains.Add(domain.DomainId);
                    // Add parent domains
                    var current = domain.Parent;
                    while (current != null)
                    {
                        allDomains.Add(current.DomainId);
                        current = current.Parent;
                    }
                }
            }

            return allDomains.Count >= 2;
        }

        /// <summary>
        /// Function that validates that a reader can borrow a maximum of NCZ books in a single day
        /// </summary>
        public bool ValidateNumberOfBorrowedBooksPerDay(IList<Book> books, Reader reader)
        {
            var limits = _configService.GetReaderLimits(reader);

            // Staff members ignore NCZ limit
            if (reader.IsEmployee) return true;

            // For now, just validate the current request count
            // TODO: Add logic to count today's borrows from database
            return books.Count <= limits.NCZ;
        }

        /// <summary>
        /// Function to validate that a user can borrow a maximum of NMC books in a period PER
        /// </summary>
        public bool ValidateBorrowedBooksLastPeriod(IList<Book> books, Reader reader)
        {
            var limits = _configService.GetReaderLimits(reader);

            // For now, just validate the current request count  
            // TODO: Add logic to count period borrows from database
            return books.Count <= limits.NMC;
        }

        /// <summary>
        /// Function to validate that a reader cannot borrow more than D books from the same domain in the last L months
        /// </summary>
        public bool ValidateBorrowedBooksDomainsTypeLastMonths(IList<Book> books, Reader reader)
        {
            var limits = _configService.GetReaderLimits(reader);

            // For now, just validate current request doesn't exceed D books from same domain
            var domainCounts = new Dictionary<int, int>();

            foreach (var book in books)
            {
                foreach (var domain in book.Domains)
                {
                    // Count this domain and all its ancestors
                    var current = domain;
                    while (current != null)
                    {
                        if (!domainCounts.ContainsKey(current.DomainId))
                            domainCounts[current.DomainId] = 0;
                        domainCounts[current.DomainId]++;

                        if (domainCounts[current.DomainId] > limits.D)
                            return false;

                        current = current.Parent;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Function to validate that a user can't borrow the same book in an interval DELTA
        /// </summary>
        public bool ValidateBorrowSameBookInPeriod(IList<Book> books, Reader reader)
        {
            // For now, just check no duplicate books in current request
            var bookIds = books.Select(b => b.BookId).ToList();
            return bookIds.Count == bookIds.Distinct().Count();

            // TODO: Add logic to check recent borrows from database
        }

        /// <summary>
        /// Function to check the sum of the extension requests of a reader in the last three months compared to the limit LIM
        /// </summary>
        public bool ValidateExtensionRequest(BorrowedBooks borrowedBook, int extensionDays)
        {
            var limits = _configService.GetReaderLimits(borrowedBook.Reader);

            // For now, just validate single extension doesn't exceed limit
            return extensionDays <= limits.LIM;

            // TODO: Add logic to sum recent extensions from database
        }
    }
}
