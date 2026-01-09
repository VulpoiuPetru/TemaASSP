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
    /// Service for validating book borrowing based on business rules and constraints
    /// </summary>
    public class BorrowBookValidation : IBorrowBookValidation
    {
        private readonly IConfigurationService _configService;
        private readonly IBorrowedBooksRepository _borrowedBooksRepository;
        private readonly IExtensionRepository _extensionRepository;

        /// <summary>
        /// Initializes a new instance of the BorrowBookValidation class
        /// </summary>
        /// <param name="configService">Configuration service</param>
        /// <param name="borrowedBooksRepository">Borrowed books repository</param>
        /// <param name="extensionRepository">Extension repository</param>
        public BorrowBookValidation(
            IConfigurationService configService,
            IBorrowedBooksRepository borrowedBooksRepository,
            IExtensionRepository extensionRepository)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _borrowedBooksRepository = borrowedBooksRepository ?? throw new ArgumentNullException(nameof(borrowedBooksRepository));
            _extensionRepository = extensionRepository ?? throw new ArgumentNullException(nameof(extensionRepository));
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

            // Count today's borrows from database
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todayBorrows = _borrowedBooksRepository.GetByReaderAndDateRange(reader.ReaderId, today, tomorrow);

            var totalTodayBorrows = todayBorrows.Count + books.Count;
            return totalTodayBorrows <= limits.NCZ;
        }

        /// <summary>
        /// Function to validate that a user can borrow a maximum of NMC books in a period PER
        /// </summary>
        public bool ValidateBorrowedBooksLastPeriod(IList<Book> books, Reader reader)
        {
            var limits = _configService.GetReaderLimits(reader);

            // Count borrows in the last PER days
            var startDate = DateTime.Now.AddDays(-limits.PER);
            var periodBorrows = _borrowedBooksRepository.GetByReaderAndDateRange(reader.ReaderId, startDate, DateTime.Now);

            var totalPeriodBorrows = periodBorrows.Count + books.Count;
            return totalPeriodBorrows <= limits.NMC;
        }

        /// <summary>
        /// Function to validate that a reader cannot borrow more than D books from the same domain in the last L months
        /// </summary>
        public bool ValidateBorrowedBooksDomainsTypeLastMonths(IList<Book> books, Reader reader)
        {
            var limits = _configService.GetReaderLimits(reader);
            var config = _configService.GetConfiguration();

            // Get borrows from last L months
            var startDate = DateTime.Now.AddMonths(-config.L);
            var recentBorrows = _borrowedBooksRepository.GetByReaderAndDateRange(reader.ReaderId, startDate, DateTime.Now);

            // Count books per domain (including current request)
            var domainCounts = new Dictionary<int, int>();

            // Count from recent borrows
            foreach (var borrow in recentBorrows)
            {
                foreach (var domain in borrow.Book.Domains)
                {
                    var current = domain;
                    while (current != null)
                    {
                        if (!domainCounts.ContainsKey(current.DomainId))
                            domainCounts[current.DomainId] = 0;
                        domainCounts[current.DomainId]++;
                        current = current.Parent;
                    }
                }
            }

            // Add current request books
            foreach (var book in books)
            {
                foreach (var domain in book.Domains)
                {
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
            var limits = _configService.GetReaderLimits(reader);

            // Check no duplicate books in current request
            var bookIds = books.Select(b => b.BookId).ToList();
            if (bookIds.Count != bookIds.Distinct().Count())
                return false;

            // Check recent borrows for each book
            var startDate = DateTime.Now.AddDays(-limits.DELTA);
            foreach (var book in books)
            {
                var recentBorrowsForBook = _borrowedBooksRepository.GetByReaderBookAndDateRange(
                    reader.ReaderId, book.BookId, startDate, DateTime.Now);

                if (recentBorrowsForBook.Any())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Function to check the sum of the extension requests of a reader in the last three months compared to the limit LIM
        /// </summary>
        public bool ValidateExtensionRequest(BorrowedBooks borrowedBook, int extensionDays)
        {
            var limits = _configService.GetReaderLimits(borrowedBook.Reader);

            // Sum extensions from last 3 months
            var totalExtensionDays = _extensionRepository.GetTotalExtensionDaysForReaderInLastMonths(
                borrowedBook.ReaderId, 3);

            // Add current extension request
            var totalWithCurrent = totalExtensionDays + extensionDays;

            return totalWithCurrent <= limits.LIM;
        }

        /// <summary>
        /// Function to validate that staff member hasn't exceeded daily lending limit (PERSIMP)
        /// </summary>
        /// <param name="staffMember">The staff member doing the lending</param>
        /// <param name="booksToLend">Number of books to lend in this transaction</param>
        /// <returns>True if within limits</returns>
        public bool ValidateStaffDailyLendingLimit(Reader staffMember, int booksToLend)
        {
            if (!staffMember.IsEmployee)
                return true; // Not staff, rule doesn't apply

            var config = _configService.GetConfiguration();

            // Count books lent TODAY by this staff member
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var todayLentByStaff = _borrowedBooksRepository
                .GetByReaderAndDateRange(staffMember.ReaderId, today, tomorrow)
                .Count;

            return (todayLentByStaff + booksToLend) <= config.PERSIMP;
        }
    }
}
