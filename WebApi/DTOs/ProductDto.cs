using WebApi.Models;

namespace WebApi.DTOs
{
	public class ProductDto
	{
		public ProductDto()
		{
			Products = new();
		}

		public List<Product> Products { get; set; }
	}
}
