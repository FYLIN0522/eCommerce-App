using Microsoft.AspNetCore.Mvc;
using MyStore.Dtos.UserDtos;
using Repositories.Abstract;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using MyStore.Services;
using Microsoft.AspNetCore.Authorization;

namespace MyStore.Controllers
{

    [ApiController]
    [Route("Products")]
    public class ProductsImageController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ImageService _imageService;

        public ProductsImageController(IProductRepository repository, ImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }

        // GET: api/Products/<id>/Image
        [HttpGet("{id}/Image")]
        public async Task<ActionResult> GetProductImage(int id)
        {
            var product = await _repository.GetProductEntityAsync(id);

            if (product is null)
            {
                return NotFound("The product does not exist");
            }

            if (product.ImageUrl is null || product.ImageUrl == "")
            {
                return NotFound("The product has no image");
            }

            var (image, mimeType) = await _imageService.ReadImage(product.ImageUrl);
            return Ok(File(image, mimeType));
        }



        // GET: api/Products/<id>/Image
        [HttpPut("{id}/Image")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetProductImage(IFormFile file, int id)
        {
            var product = await _repository.GetProductEntityAsync(id);
            bool isNew = false;

            if (product is null)
            {
                return NotFound("The product does not exist");
            }

            if (file.Length < 1)
            {
                return StatusCode(400, "Bad request: empty image");
            }

            var mineType = file.ContentType;
            var fileExt = _imageService.GetImageExtension(mineType);
            if (fileExt is null)
            {
                return StatusCode(400, $"Bad Request: photo must be image/jpeg, image/png, image/gif type, but it was: {mineType}");
            }

            if (product.ImageUrl is null || product.ImageUrl == "") isNew = true;

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] fileBytes = ms.ToArray();

            var fileName = await _imageService.AddImage(fileBytes, fileExt);
            product.ImageUrl = fileName;
            await _repository.UpdateProductAsync(product);

            if (isNew)
            {
                return StatusCode(201, "Created Image added");
            }
            else 
            {
                return Ok("OK. Image updated");
            }
        }
    }
}
