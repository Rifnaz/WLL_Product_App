using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
	public class Cart
	{
		[Key]
		public int Id { get; set; }

		public int ProductId { get; set; }

		public int Quantity { get; set; }

		[NotMapped]
		public Product? Product { get; set; }
	}
}
