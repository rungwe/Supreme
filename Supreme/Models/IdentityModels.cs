using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Supreme.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Supreme.Models.Profile> Profiles { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Accountant> Accountants { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Administrator> Administrators { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Branch> Branches { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.DeliveryNote> DeliveryNotes { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Dispatch> Dispatches { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Order> Orders { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.StockManager> StockManagers { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Driver> Drivers { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Invoice> Invoices { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Merchant> Merchants { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.SalesRep> SalesReps { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.ProductPrice> ProductPrices { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Region> Regions { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Return> Returns { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.OrderProduct> OrderProducts { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Bank> Banks { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.BranchProductPrice> BranchProductPrices { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.Vat> Vats { get; set; }

        public System.Data.Entity.DbSet<Supreme.Models.SalesLedger> SalesLedgers { get; set; }
    }
}