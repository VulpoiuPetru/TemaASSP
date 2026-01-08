using DomainModel;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ServiceServiceCollectionMethod
{
    public static class ServiceServiceCollection
    {
        public static IServiceCollection AddServiceLayerServices(this IServiceCollection services)
        {
            // Register validators
            services.AddTransient<IValidator<Author>, AuthorValidator>();
            services.AddTransient<IValidator<Book>, BookValidator>();
            services.AddTransient<IValidator<Copy>, CopyValidator>();
            services.AddTransient<IValidator<Domain>, DomainValidator>();
            services.AddTransient<IValidator<Edition>, EditionValidator>();
            services.AddTransient<IValidator<Extension>, ExtensionValidator>();
            services.AddTransient<IValidator<Reader>, ReaderValidator>();



            // Register main services
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBorrowedBookService, BorrowedBookService>();
            services.AddScoped<IBorrowBookValidation, BorrowBookValidation>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<ICopyService, CopyService>();
            services.AddScoped<IDomainService, DomainService>();
            services.AddScoped<IEditionService, EditionService>();
            services.AddScoped<IExtensionService, ExtensionService>();
            services.AddScoped<IReaderService, ReaderService>();

            return services;
        }
    }
}
