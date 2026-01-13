using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DataMapper
{
    /// <summary>
    /// Database context for the library management system.
    /// </summary>
    public class LibraryContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryContext"/> class.
        /// </summary>
        public LibraryContext() : base("name=LibraryDBConnectionString")
        {
            //create the database if it does not exist
            Database.SetInitializer(new CreateDatabaseIfNotExists<LibraryContext>());
        }

        /// <summary>
        /// Gets or sets the Authors table.
        /// </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary>
        /// Gets or sets the Books table.
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Gets or sets the BorrowedBooks table.
        /// </summary>
        public DbSet<BorrowedBooks> BorrowedBooks { get; set; }

        /// <summary>
        /// Gets or sets the Copies table.
        /// </summary>
        public DbSet<Copy> Copies { get; set; }

        /// <summary>
        /// Gets or sets the Domains table.
        /// </summary>
        public DbSet<Domain> Domains { get; set; }

        /// <summary>
        /// Gets or sets the Editions table.
        /// </summary>
        public DbSet<Edition> Editions { get; set; }

        /// <summary>
        /// Gets or sets the Extensions table.
        /// </summary>
        public DbSet<Extension> Extensions { get; set; }

        /// <summary>
        /// Gets or sets the Readers table.
        /// </summary>
        public DbSet<Reader> Readers { get; set; }

        /// <summary>
        /// Configures the model relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Books and Authors
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Authors)
                .WithMany(a => a.Books)
                .Map(m =>
                {
                    m.ToTable("BookAuthors");
                    m.MapLeftKey("BookId");
                    m.MapRightKey("AuthorId");
                });

            // Configure many-to-many relationship between Books and Domains
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Domains)
                .WithMany(d => d.Books)
                .Map(m =>
                {
                    m.ToTable("BookDomains");
                    m.MapLeftKey("BookId");
                    m.MapRightKey("DomainId");
                });

            // Configure one-to-one relationship between Book and Edition
            modelBuilder.Entity<Book>()
                .HasRequired(b => b.Edition)
                .WithRequiredPrincipal(e => e.Book);

            // Configure composite key for BorrowedBooks
            modelBuilder.Entity<BorrowedBooks>()
                .HasKey(bb => new { bb.BookId, bb.ReaderId });

            // Disable cascade delete for BorrowedBooks->Reader
            
                        modelBuilder.Entity<BorrowedBooks>()
                .HasRequired(bb => bb.Reader)
                .WithMany(r => r.BorrowedBooks)
                .HasForeignKey(bb => bb.ReaderId)
                .WillCascadeOnDelete(false);

            // Disable cascade delete for BorrowedBooks -> Book
            modelBuilder.Entity<BorrowedBooks>()
                .HasRequired(bb => bb.Book)
                .WithMany(b => b.BorrowedBooks)
                .HasForeignKey(bb => bb.BookId)
                .WillCascadeOnDelete(false);

            // Configure self-referencing relationship for Domain (Parent-Child)
            modelBuilder.Entity<Domain>()
                .HasOptional(d => d.Parent)
                .WithMany()
                .Map(m => m.MapKey("ParentDomainId"));

            // Configure relationship between Copy and Edition - DISABLE CASCADE DELETE
            modelBuilder.Entity<Copy>()
                .HasRequired(c => c.Edition)
                .WithMany(e => e.Copies)
                .WillCascadeOnDelete(false);


            // Extension -> BorrowedBooks (using composite keys BookId and ReaderId)
            modelBuilder.Entity<Extension>()
                .HasRequired(ext => ext.BorrowedBooks)
                .WithMany(bb => bb.Extensions)
                .HasForeignKey(ext => new { ext.BookId, ext.ReaderId })
                .WillCascadeOnDelete(false);
        }
    }
}
