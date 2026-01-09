using DataMapper.RepoInterfaces;
using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceLayer;
using ServiceLayer.ServiceConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestServiceLayer
{

    /// <summary>
    /// Tests for the BorrowBookValidation class
    /// </summary>
    [TestClass]
    public class BorrowBookValidationTests
    {
        private Mock<IConfigurationService> _mockConfigService;
        private Mock<IBorrowedBooksRepository> _mockBorrowedBooksRepository;
        private Mock<IExtensionRepository> _mockExtensionRepository;
        private BorrowBookValidation _borrowValidation;
        private LibraryConfiguration _config;
        private ReaderLimits _readerLimits;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockConfigService = new Mock<IConfigurationService>();
            _mockBorrowedBooksRepository = new Mock<IBorrowedBooksRepository>();
            _mockExtensionRepository = new Mock<IExtensionRepository>();

            _config = new LibraryConfiguration
            {
                DOMENII = 3,
                NMC = 10,
                PER = 30,
                C = 5,
                D = 3,
                L = 6,
                LIM = 30,
                DELTA = 14,
                NCZ = 2,
                PERSIMP = 20
            };

            _readerLimits = new ReaderLimits
            {
                NMC = 10,
                PER = 30,
                C = 5,
                D = 3,
                LIM = 30,
                DELTA = 14,
                NCZ = 2
            };

            _mockConfigService.Setup(c => c.GetConfiguration()).Returns(_config);
            _mockConfigService.Setup(c => c.GetReaderLimits(It.IsAny<Reader>())).Returns(_readerLimits);

            _borrowValidation = new BorrowBookValidation(
                _mockConfigService.Object,
                _mockBorrowedBooksRepository.Object,
                _mockExtensionRepository.Object);
        }

        [TestMethod]
        public void TestValidateIfBookCanBeBorrowed_AllCopiesReadingRoom_ReturnsFalse()
        {
            var book = new Book
            {
                NumberOfTotalBooks = 10,
                NumberOfReadingRoomBooks = 10
            };

            var result = _borrowValidation.ValidateIfBookCanBeBorrowed(book);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateIfBookCanBeBorrowed_SomeBorrowableCopies_ReturnsTrue()
        {
            var book = new Book
            {
                NumberOfTotalBooks = 10,
                NumberOfReadingRoomBooks = 5
            };

            var result = _borrowValidation.ValidateIfBookCanBeBorrowed(book);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateIfThereAreAvailableCopiesToBorrow_BelowTenPercent_ReturnsFalse()
        {
            var book = new Book
            {
                NumberOfTotalBooks = 100,
                NumberOfReadingRoomBooks = 0,
                NumberOfAvailableBooks = 5 // Less than 10%
            };

            var result = _borrowValidation.ValidateIfThereAreAvailableCopiesToBorrow(book);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateIfThereAreAvailableCopiesToBorrow_AtTenPercent_ReturnsTrue()
        {
            var book = new Book
            {
                NumberOfTotalBooks = 100,
                NumberOfReadingRoomBooks = 0,
                NumberOfAvailableBooks = 10 // Exactly 10%
            };

            var result = _borrowValidation.ValidateIfThereAreAvailableCopiesToBorrow(book);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateIfThereAreAvailableCopiesToBorrow_AboveTenPercent_ReturnsTrue()
        {
            var book = new Book
            {
                NumberOfTotalBooks = 100,
                NumberOfReadingRoomBooks = 0,
                NumberOfAvailableBooks = 20 // Above 10%
            };

            var result = _borrowValidation.ValidateIfThereAreAvailableCopiesToBorrow(book);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateNumberOfBorrowedBooks_WithinLimit_ReturnsTrue()
        {
            var books = new List<Book> { new Book(), new Book(), new Book() };
            var reader = new Reader { IsEmployee = false };

            var result = _borrowValidation.ValidateNumberOfBorrowedBooks(books, reader);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateNumberOfBorrowedBooks_ExceedsLimit_ReturnsFalse()
        {
            var books = new List<Book> { new Book(), new Book(), new Book(), new Book(), new Book(), new Book() };
            var reader = new Reader { IsEmployee = false };

            var result = _borrowValidation.ValidateNumberOfBorrowedBooks(books, reader);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateDomainsForMoreThanThreeBooks_LessThanThree_ReturnsTrue()
        {
            var books = new List<Book>
            {
                new Book { Domains = new List<Domain> { new Domain { DomainId = 1 } } },
                new Book { Domains = new List<Domain> { new Domain { DomainId = 1 } } }
            };

            var result = _borrowValidation.ValidateDomainsForMoreThanThreeBooks(books);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateDomainsForMoreThanThreeBooks_ThreeBooksOneDomain_ReturnsFalse()
        {
            var domain = new Domain { DomainId = 1, Parent = null };
            var books = new List<Book>
            {
                new Book { Domains = new List<Domain> { domain } },
                new Book { Domains = new List<Domain> { domain } },
                new Book { Domains = new List<Domain> { domain } }
            };

            var result = _borrowValidation.ValidateDomainsForMoreThanThreeBooks(books);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateDomainsForMoreThanThreeBooks_ThreeBooksMultipleDomains_ReturnsTrue()
        {
            var domain1 = new Domain { DomainId = 1, Parent = null };
            var domain2 = new Domain { DomainId = 2, Parent = null };
            var books = new List<Book>
            {
                new Book { Domains = new List<Domain> { domain1 } },
                new Book { Domains = new List<Domain> { domain1 } },
                new Book { Domains = new List<Domain> { domain2 } }
            };

            var result = _borrowValidation.ValidateDomainsForMoreThanThreeBooks(books);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateNumberOfBorrowedBooksPerDay_RegularReader_WithinLimit_ReturnsTrue()
        {
            var books = new List<Book> { new Book() };
            var reader = new Reader { ReaderId = 1, IsEmployee = false };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<BorrowedBooks>());

            var result = _borrowValidation.ValidateNumberOfBorrowedBooksPerDay(books, reader);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateNumberOfBorrowedBooksPerDay_RegularReader_ExceedsLimit_ReturnsFalse()
        {
            var books = new List<Book> { new Book() };
            var reader = new Reader { ReaderId = 1, IsEmployee = false };

            var todayBorrows = new List<BorrowedBooks>
            {
                new BorrowedBooks(),
                new BorrowedBooks()
            };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(todayBorrows);

            var result = _borrowValidation.ValidateNumberOfBorrowedBooksPerDay(books, reader);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateNumberOfBorrowedBooksPerDay_StaffMember_IgnoresLimit_ReturnsTrue()
        {
            var books = new List<Book> { new Book(), new Book(), new Book() };
            var reader = new Reader { ReaderId = 1, IsEmployee = true };

            var result = _borrowValidation.ValidateNumberOfBorrowedBooksPerDay(books, reader);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateBorrowedBooksLastPeriod_WithinLimit_ReturnsTrue()
        {
            var books = new List<Book> { new Book() };
            var reader = new Reader { ReaderId = 1 };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<BorrowedBooks>());

            var result = _borrowValidation.ValidateBorrowedBooksLastPeriod(books, reader);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateBorrowedBooksLastPeriod_ExceedsLimit_ReturnsFalse()
        {
            var books = new List<Book> { new Book() };
            var reader = new Reader { ReaderId = 1 };

            var periodBorrows = Enumerable.Range(0, 10).Select(i => new BorrowedBooks()).ToList();

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(periodBorrows);

            var result = _borrowValidation.ValidateBorrowedBooksLastPeriod(books, reader);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateBorrowedBooksDomainsTypeLastMonths_WithinLimit_ReturnsTrue()
        {
            var domain = new Domain { DomainId = 1, Parent = null };
            var books = new List<Book>
            {
                new Book { Domains = new List<Domain> { domain } }
            };
            var reader = new Reader { ReaderId = 1 };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<BorrowedBooks>());

            var result = _borrowValidation.ValidateBorrowedBooksDomainsTypeLastMonths(books, reader);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateBorrowedBooksDomainsTypeLastMonths_ExceedsLimit_ReturnsFalse()
        {
            var domain = new Domain { DomainId = 1, Parent = null };
            var books = new List<Book>
            {
                new Book { Domains = new List<Domain> { domain } }
            };
            var reader = new Reader { ReaderId = 1 };

            var recentBorrows = Enumerable.Range(0, 3).Select(i => new BorrowedBooks
            {
                Book = new Book { Domains = new List<Domain> { domain } }
            }).ToList();

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(recentBorrows);

            var result = _borrowValidation.ValidateBorrowedBooksDomainsTypeLastMonths(books, reader);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateBorrowSameBookInPeriod_DuplicateInRequest_ReturnsFalse()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1 },
                new Book { BookId = 1 }
            };
            var reader = new Reader { ReaderId = 1 };

            var result = _borrowValidation.ValidateBorrowSameBookInPeriod(books, reader);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateBorrowSameBookInPeriod_RecentlyBorrowed_ReturnsFalse()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1 }
            };
            var reader = new Reader { ReaderId = 1 };

            var recentBorrow = new List<BorrowedBooks>
            {
                new BorrowedBooks { BookId = 1, ReaderId = 1 }
            };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderBookAndDateRange(1, 1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(recentBorrow);

            var result = _borrowValidation.ValidateBorrowSameBookInPeriod(books, reader);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateBorrowSameBookInPeriod_NotRecentlyBorrowed_ReturnsTrue()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1 }
            };
            var reader = new Reader { ReaderId = 1 };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderBookAndDateRange(1, 1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<BorrowedBooks>());

            var result = _borrowValidation.ValidateBorrowSameBookInPeriod(books, reader);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateExtensionRequest_WithinLimit_ReturnsTrue()
        {
            var borrowedBook = new BorrowedBooks
            {
                ReaderId = 1,
                Reader = new Reader { IsEmployee = false }
            };

            _mockExtensionRepository
                .Setup(r => r.GetTotalExtensionDaysForReaderInLastMonths(1, 3))
                .Returns(10);

            var result = _borrowValidation.ValidateExtensionRequest(borrowedBook, 10);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateExtensionRequest_ExceedsLimit_ReturnsFalse()
        {
            var borrowedBook = new BorrowedBooks
            {
                ReaderId = 1,
                Reader = new Reader { IsEmployee = false }
            };

            _mockExtensionRepository
                .Setup(r => r.GetTotalExtensionDaysForReaderInLastMonths(1, 3))
                .Returns(25);

            var result = _borrowValidation.ValidateExtensionRequest(borrowedBook, 10);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestValidateStaffDailyLendingLimit_NotStaff_ReturnsTrue()
        {
            var reader = new Reader { ReaderId = 1, IsEmployee = false };

            var result = _borrowValidation.ValidateStaffDailyLendingLimit(reader, 100);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateStaffDailyLendingLimit_WithinLimit_ReturnsTrue()
        {
            var reader = new Reader { ReaderId = 1, IsEmployee = true };

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new List<BorrowedBooks>());

            var result = _borrowValidation.ValidateStaffDailyLendingLimit(reader, 10);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidateStaffDailyLendingLimit_ExceedsLimit_ReturnsFalse()
        {
            var reader = new Reader { ReaderId = 1, IsEmployee = true };

            var todayLent = Enumerable.Range(0, 15).Select(i => new BorrowedBooks()).ToList();

            _mockBorrowedBooksRepository
                .Setup(r => r.GetByReaderAndDateRange(1, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(todayLent);

            var result = _borrowValidation.ValidateStaffDailyLendingLimit(reader, 10);

            Assert.IsFalse(result);
        }
    }
}
