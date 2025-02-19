using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class StoreContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public required DbSet<Product> Products { get; set; }
    public required DbSet<Basket> Baskets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>()
        .HasData(
            new IdentityRole { Id = "cca9c1d2-3e77-4015-babe-bad9311b422f", Name = "Member", NormalizedName = "MEMBER" },
            new IdentityRole { Id = "de2d9ecf-8090-4e1c-861d-e30c0470b296", Name = "Admin", NormalizedName = "ADMIN" }
        );
    }
}
