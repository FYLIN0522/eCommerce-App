using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyStore.Data;
using MyStore.Models;
using Repositories.Abstract;
using MyStore.Dtos.ProductDtos;


namespace Repositories.Concrete
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResponseProductDto>> GetAllProducts(ProductQuery Query)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking();

            if (Query.Q != null)
            {
                string lowerQ = Query.Q.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(lowerQ)
                 || (p.Description != null && p.Description.ToLower().Contains(lowerQ)));
            }

            if (Query.CategoryIds != 0)
            {
                query = query.Where(p => p.CategoryId == Query.CategoryIds);
            }

            if (Query.minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= Query.minPrice.Value);
            }

            if (Query.maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= Query.maxPrice.Value);
            }


            string sortBy = Query.SortBy != null ? Query.SortBy : "ALPHABETICAL_ASC";
            switch (sortBy)
            {
                case "ALPHABETICAL_ASC":
                    query = query.OrderBy(p => p.Name)
                                    .ThenBy(p => p.ProductId);
                    break;

                case "ALPHABETICAL_DESC":
                    query = query.OrderByDescending(p => p.Name)
                                    .ThenBy(p => p.ProductId);
                    break;

                case "COST_ASC":
                    query = query.OrderBy(p => p.Price)
                                    .ThenBy(p => p.ProductId);
                    break;

                case "COST_DESC":
                    query = query.OrderByDescending(p => p.Price)
                                    .ThenBy(p => p.ProductId);
                    break;

                case "STOCKQUANTITY_DESC":
                    query = query.OrderByDescending(p => p.StockQuantity)
                                    .ThenBy(p => p.ProductId);
                    break;

                default:
                    break;
            }

            query = query.Include(p => p.Category);

            var ProduceDetails = query.Select(p => new ResponseProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category
            });

            return await ProduceDetails.ToListAsync();
        }

        public async Task<ResponseProductDto?> GetProductId(int id)
        {

            return await _context.Products
                            .Include(p => p.Category)
                            .AsNoTracking()
                            .Select(p => new ResponseProductDto
                            {
                                ProductId = p.ProductId,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                StockQuantity = p.StockQuantity,
                                Category = p.Category
                            })
                            .SingleOrDefaultAsync(p => p.ProductId == id);
        }




        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsWithNameAsync(string title)
        {
            return await _context.Products.AnyAsync(p => p.Name == title);
        }

        public async Task<bool> CheckCategoryAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(p => p.CategoryId == categoryId);
        }

        public async Task<Product?> GetProductEntityAsync(int productId)
        {
            //return await _context.Products.AnyAsync(p => p.ProductId == productId);
            return await _context.Products
                    .Include(p => p.Category)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.ProductId == productId);
        }


        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is not null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
