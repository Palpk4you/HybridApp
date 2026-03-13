using HybridApp.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System.Data;

    namespace HybridApp.Data { 

    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Role>().HasData(
                new Role
                {
                    //Id = Guid.NewGuid().ToString(),
                    //Name = "Super Admin",
                    //NormalizedName = "SUPER ADMIN",
                    //Description = "Super Admin role",
                    //ConcurrencyStamp = Guid.NewGuid().ToString()
                    Id = "b74ddd14-6340-4840-95c2-db12554843e5", // static GUID
                    Name = "Super Admin",
                    NormalizedName = "SUPER ADMIN",
                    Description = "Super Admin role",
                    ConcurrencyStamp = "1" // can be any static string
//INSERT INTO [dbo].[AspNetRoles]
//([Id], [Description], [Name], [NormalizedName], [ConcurrencyStamp])
//VALUES
//(NEWID(), 'Super Admin role', 'Super Admin', 'SUPER ADMIN', NEWID());
                }
                //,
                //new Role
                //{
                //    Id = Guid.NewGuid().ToString(),
                //    Name = "User",
                //    NormalizedName = "USER",
                //    Description = "Default user role",
                //    ConcurrencyStamp = Guid.NewGuid().ToString()
                //}
            );
        }


    }
}
