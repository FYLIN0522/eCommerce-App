using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

using MyStore.Models;
using Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using MyStore.Dtos.ProductDtos;



namespace MyStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts([FromQuery] ProductQuery productQuery)
        {
            //try{
            //} catch (Exception ex){
            //    return StatusCode(500, "Internal Server Error.");
            //}
            var products = await _repository.GetAllProducts(productQuery);
            return Ok(products);
        }

        // GET: api/Product/<id>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _repository.GetProductId(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }



        // POST: api/Product
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto newProduct)
        {
            //StatusCode: 403 
            bool nameExists = await _repository.ExistsWithNameAsync(newProduct.Name);
            if (nameExists)
            {
                //return Forbid("Product name already exists!");
                return StatusCode(403, "Product name already exists!");
            }

            //StatusCode: 404
            bool categoryExists = await _repository.CheckCategoryAsync(newProduct.CategoryId);
            if (!categoryExists)
            {
                return NotFound("ID for category is not exists!");
            }

            var newProductReq = new Product
            {
                Name = newProduct.Name,
                Description = newProduct.Description,
                Price = newProduct.Price,
                StockQuantity = newProduct.StockQuantity,
                CategoryId = newProduct.CategoryId
            };

            await _repository.AddProductAsync(newProductReq);
            return CreatedAtAction(nameof(GetProduct), new { id = newProductReq.ProductId }, newProductReq);
        }


        // PATCH: api/Product/<id>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto productReq)
        {

            try
            {
                var productEntity = await _repository.GetProductEntityAsync(id);

                if (productEntity is null)
                {
                    return NotFound("Not Found. No product found with id");
                }

                //PUT Request body
                if (productReq.Name is not null)
                {
                    productEntity.Name = productReq.Name;

                    //StatusCode: 403 
                    bool nameExists = await _repository.ExistsWithNameAsync(productEntity.Name);
                    if (nameExists)
                    {
                        return StatusCode(403, "Product name already exists!");
                    }
                }
                if (productReq.Description is not null)
                {
                    productEntity.Description = productReq.Description;
                }
                if (productReq.Price.HasValue)
                {
                    productEntity.Price = productReq.Price.Value;
                }
                if (productReq.StockQuantity.HasValue)
                {
                    productEntity.StockQuantity = productReq.StockQuantity.Value;
                }
                if (productReq.CategoryId.HasValue)
                {
                    productEntity.CategoryId = productReq.CategoryId.Value;
                }

                //StatusCode: 404 
                bool categoryExists = await _repository.CheckCategoryAsync(productEntity.CategoryId);
                if (!categoryExists)
                {
                    return NotFound("ID for category is not exists!");
                }


                await _repository.UpdateProductAsync(productEntity);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }


        // DELETE: api/Product/<id>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetProductId(id);
            if (product is null)
            {
                return NotFound("Not Found. No product found with id");
            }

            await _repository.DeleteProductAsync(id);

            return Ok();
        }
    }
}
