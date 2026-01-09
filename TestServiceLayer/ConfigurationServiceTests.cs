using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ServiceLayer;
using DomainModel;

namespace TestServiceLayer
{

    /// <summary>
    /// Tests for the ConfigurationService class
    /// </summary>
    [TestClass]
    public class ConfigurationServiceTests
    {
        private ConfigurationService _configService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _configService = new ConfigurationService();
        }

        [TestMethod]
        public void TestGetConfiguration_ReturnsValidConfiguration()
        {
            var config = _configService.GetConfiguration();

            Assert.IsNotNull(config);
            Assert.IsTrue(config.DOMENII > 0);
            Assert.IsTrue(config.NMC > 0);
            Assert.IsTrue(config.PER > 0);
            Assert.IsTrue(config.C > 0);
            Assert.IsTrue(config.D > 0);
            Assert.IsTrue(config.L > 0);
            Assert.IsTrue(config.LIM > 0);
            Assert.IsTrue(config.DELTA > 0);
            Assert.IsTrue(config.NCZ > 0);
            Assert.IsTrue(config.PERSIMP > 0);
        }

        [TestMethod]
        public void TestGetReaderLimits_ForRegularReader_ReturnsBaseLimits()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Age = 25,
                Email = "alice@test.com",
                IsEmployee = false
            };

            var limits = _configService.GetReaderLimits(reader);
            var config = _configService.GetConfiguration();

            Assert.AreEqual(config.NMC, limits.NMC);
            Assert.AreEqual(config.C, limits.C);
            Assert.AreEqual(config.D, limits.D);
            Assert.AreEqual(config.LIM, limits.LIM);
            Assert.AreEqual(config.DELTA, limits.DELTA);
            Assert.AreEqual(config.PER, limits.PER);
            Assert.AreEqual(config.NCZ, limits.NCZ);
        }

        [TestMethod]
        public void TestGetReaderLimits_ForStaffMember_ReturnsDoubledLimits()
        {
            var staff = new Reader
            {
                ReaderId = 2,
                FirstName = "Bob",
                LastName = "Staff",
                Age = 30,
                Email = "bob@library.com",
                IsEmployee = true
            };

            var limits = _configService.GetReaderLimits(staff);
            var config = _configService.GetConfiguration();

            Assert.AreEqual(config.NMC * 2, limits.NMC);
            Assert.AreEqual(config.C * 2, limits.C);
            Assert.AreEqual(config.D * 2, limits.D);
            Assert.AreEqual(config.LIM * 2, limits.LIM);
        }

        [TestMethod]
        public void TestGetReaderLimits_ForStaffMember_ReturnsHalvedPeriods()
        {
            var staff = new Reader
            {
                ReaderId = 2,
                FirstName = "Bob",
                LastName = "Staff",
                Age = 30,
                Email = "bob@library.com",
                IsEmployee = true
            };

            var limits = _configService.GetReaderLimits(staff);
            var config = _configService.GetConfiguration();

            Assert.AreEqual(config.DELTA / 2, limits.DELTA);
            Assert.AreEqual(config.PER / 2, limits.PER);
        }

        [TestMethod]
        public void TestGetReaderLimits_ForStaffMember_IgnoresNCZ()
        {
            var staff = new Reader
            {
                ReaderId = 2,
                FirstName = "Bob",
                LastName = "Staff",
                Age = 30,
                Email = "bob@library.com",
                IsEmployee = true
            };

            var limits = _configService.GetReaderLimits(staff);

            Assert.AreEqual(int.MaxValue, limits.NCZ);
        }

        [TestMethod]
        public void TestGetConfiguration_ReturnsSameInstanceOnMultipleCalls()
        {
            var config1 = _configService.GetConfiguration();
            var config2 = _configService.GetConfiguration();

            Assert.AreSame(config1, config2);
        }

        [TestMethod]
        public void TestConfiguration_DOMENIIIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.DOMENII > 0);
        }

        [TestMethod]
        public void TestConfiguration_NMCIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.NMC > 0);
        }

        [TestMethod]
        public void TestConfiguration_PERIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.PER > 0);
        }

        [TestMethod]
        public void TestConfiguration_CIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.C > 0);
        }

        [TestMethod]
        public void TestConfiguration_DIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.D > 0);
        }

        [TestMethod]
        public void TestConfiguration_LIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.L > 0);
        }

        [TestMethod]
        public void TestConfiguration_LIMIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.LIM > 0);
        }

        [TestMethod]
        public void TestConfiguration_DELTAIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.DELTA > 0);
        }

        [TestMethod]
        public void TestConfiguration_NCZIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.NCZ > 0);
        }

        [TestMethod]
        public void TestConfiguration_PERSIMPIsPositive()
        {
            var config = _configService.GetConfiguration();
            Assert.IsTrue(config.PERSIMP > 0);
        }
    }
}
