using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApi.DTOs;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly HttpClient _httpClient;

		public HomeController(AppDbContext context, HttpClient httpClient)
		{
			_context    = context;
			_httpClient = httpClient;
		}

		/// <summary>
		/// Fetches all products from the external API and returns them as JSON.
		/// </summary>
		/// <returns></returns>
		[HttpGet("products")]
		public async Task<IActionResult> GetAllProducts()
		{
			string ApiUrl = "https://dummyjson.com/products";

			var products = await GetProducts();

			if(products == null || !products.Any())
				BadRequest("Unable to fetch the products.");

			return Ok(products);
		}

		/// <summary>
		/// Fet product details by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("products/{id}")]
		public async Task<IActionResult> GetProductById(int id)
		{
			string ApiUrl = $"https://dummyjson.com/products/{id}";

			try
			{
				var response = await _httpClient.GetAsync(ApiUrl);

				if(!response.IsSuccessStatusCode)
					BadRequest("Unable to get the product details.");

				var content = await response.Content.ReadAsStringAsync();
				var options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true};

				var result = JsonSerializer.Deserialize<Product>(content, options) ?? new();

				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Add Product to the cart
		/// </summary>
		/// <param name="cart"></param>
		/// <returns></returns>
		[HttpPost("AddToCart")]
		public async Task<IActionResult> AddToCart([FromBody] Cart cart)
		{
			if (cart == null)
				return BadRequest("Invalid Cart Item.");

			if (cart.ProductId <= 0)
				return BadRequest("Invalid Prodcut Id.");

			if (cart.Quantity <= 0)
				return BadRequest("At least 1 quantity should be added.");

			try
			{
				var existItem = _context.Carts.FirstOrDefault(x => x.ProductId == cart.ProductId);

				if (existItem != null)
				{
					 existItem.Quantity += cart.Quantity;

					_context.Carts.Update(existItem);
					await _context.SaveChangesAsync();

					return Ok("Updated.");
				}
				else
				{
					await _context.Carts.AddAsync(cart);
					await _context.SaveChangesAsync();

					return Ok("Added.");
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Add Product to the cart
		/// </summary>
		/// <param name="cart"></param>
		/// <returns></returns>
		[HttpDelete("DeleteFromCart/{id}")]
		public async Task<IActionResult> DeleteFromCart(int id)
		{

			if (id <= 0)
				return BadRequest("Invalid Prodcut Id.");

			var existItem = _context.Carts.FirstOrDefault(x => x.Id == id);

			if(existItem == null)
				return BadRequest("Item Not Found.");

			try
			{
				_context.Carts.Remove(existItem);
				await _context.SaveChangesAsync();

				return Ok("Removed.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Get Cart items
		/// </summary>
		/// <returns></returns>
		[HttpGet("Cart")]
		public async Task<IActionResult> GetItemsFromCart()
		{
			var products = await GetProducts();

			if (products == null)
				return BadRequest("Something went wrong.");

			var cartItems = await _context.Carts.ToListAsync();

			try
			{
				var items = (from c in cartItems
							 join p in products
							 on c.ProductId equals p.Id
							 select new
							 {
								 c.Id,
								 c.ProductId,
								 c.Quantity,
								 Product = p
							 }).ToList();

				return Ok(items);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Get All product list
		/// </summary>
		/// <returns></returns>
		private async Task<List<Product>> GetProducts()
		{
			string ApiUrl = "https://dummyjson.com/products";

			try
			{
				var response = await _httpClient.GetAsync(ApiUrl);

				if (!response.IsSuccessStatusCode)
					return new();

				var content = await response.Content.ReadAsStringAsync();
				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

				var result = JsonSerializer.Deserialize<ProductDto>(content, options) ?? new();

				return result.Products;
			}
			catch (Exception ex)
			{
				return new();
			}
		}
	}
}
