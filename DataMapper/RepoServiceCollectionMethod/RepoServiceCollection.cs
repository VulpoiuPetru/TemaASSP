using DataMapper.RepoInterfaces;
using DataMapper.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoServiceCollectionMethod
{
    public static class RepoServiceCollection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {

            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBorrowedBooksRepository, BorrowedBooksRepository>();
            services.AddScoped<ICopyRepository, CopyRepository>();
            services.AddScoped<IDomainRepository, DomainRepository>();
            services.AddScoped<IEditionRepository, EditionRepository>();
            services.AddScoped<IExtensionRepository, ExtensionRepository>();
            services.AddScoped<IReaderRepository, ReaderRepository>();

            services.AddMemoryCache();
            return services;
        }
    }
}
