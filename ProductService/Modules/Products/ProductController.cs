using Common.Pagination;
using Common.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductService.Modules.Products.Dto;
using ProductService.Modules.Products.Model;
using ProductService.Modules.Products.Service;
using System.Security.Claims;

namespace ProductService.Modules.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService, IUploadService uploadService)
        : ControllerBase
    {
        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product?>> Get(string id)
        {
            var product = await productService.GetAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponse>>> GetList(
            [FromQuery] PagedRequest request
        )
        {
            var result = await productService.GetListAsync(request);
            return Ok(result);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductRequest postDto)
        {
            
            if (string.IsNullOrEmpty(postDto.Id))
            {
                var product = new Product()
                {
                    Name = postDto.Name,
                    Category = postDto.Category,
                    Status = postDto.Status,
                    Imgs = postDto.Imgs,
                    Count = postDto.Count,
                    Price = postDto.Price,
                };
                productService.Create(product);
            }
            else
            {
                var product = await productService.GetAsync(postDto.Id);
                if(product == null)
                    return NotFound("产品不存在");
                product.UpdateFrom(postDto);
            }
            await productService.SaveChangesAsync();
            return Ok("保存成功");
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(string id)
        {
            var product = await productService.GetAsync(id);
            if (product == null)
                return NotFound();

            productService.Remove(product);
            await productService.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var url = await uploadService.UploadAsync(file);
            return Ok(new { url });
        }

        [HttpGet("categories")]
        public ActionResult<Dictionary<int, string>> GetProductCategories()
        {
            var values = Enum.GetValues(typeof(ProductCategory))
                .Cast<ProductCategory>()
                .Select(x => new { id = (int)x, name = x.ToString() });
            return Ok(values);
        }
        [HttpPost("{id}/Seckill")]
        public async Task<ActionResult<ProductResponse>> Seckill([FromRoute]string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized("用户未登录");
            }
            var result = await
                productService.Seckill(id,userId);
            return Ok(result);
        }
    }
}
