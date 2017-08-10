using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CmsSite.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet <CmsPage> CmsPages { get; set; }
        public DbSet<CmsProperty> CmsProperties { get; set; }
        public DbSet<CmsTemplate> CmsTemplates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CmsPage>()
                .HasOptional(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.ParentId);
            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       
    }
}