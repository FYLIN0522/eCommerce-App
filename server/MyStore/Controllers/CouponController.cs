using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyStore.Context;
using MyStore.Models;


namespace MyStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CouponController
    {
        PromotionsContext _context;

        public CouponController(PromotionsContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Coupon> Get()
        {
            return _context.Coupons
                .AsNoTracking()
                .ToList();
        }
    }
}
