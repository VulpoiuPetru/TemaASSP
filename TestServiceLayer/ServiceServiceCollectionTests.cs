using DomainModel;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.ServiceServiceCollectionMethod;
using System;

namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="ServiceServiceCollection"/>
    /// </summary>
    [TestClass]
    public class ServiceServiceCollectionTests
    {
        /// <summary>
        /// All validators should be registered correctly in DI container
        /// </summary>
        [TestMethod]
        public void AddServiceLayerServices_ShouldRegisterValidators()
        {
            var services = new ServiceCollection();

            services.AddServiceLayerServices();

            var provider = services.BuildServiceProvider();

            Assert.IsNotNull(provider.GetService<IValidator<Author>>());
            Assert.IsNotNull(provider.GetService<IValidator<Book>>());
            Assert.IsNotNull(provider.GetService<IValidator<Copy>>());
            Assert.IsNotNull(provider.GetService<IValidator<Domain>>());
            Assert.IsNotNull(provider.GetService<IValidator<Edition>>());
            Assert.IsNotNull(provider.GetService<IValidator<Extension>>());
            Assert.IsNotNull(provider.GetService<IValidator<Reader>>());
        }
    }
}
