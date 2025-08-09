using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{			
		}

		public DbSet<Product> Products { get; set; }

		public DbSet<Cart> Carts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
