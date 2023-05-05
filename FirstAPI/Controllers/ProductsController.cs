//using FirstAPI.Data;
//using FirstAPI.Extensions;
//using FirstAPI.Models;
//using FirstAPI.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace FirstAPI.Controllers
//{
//    [ApiController]

//    public class ProductsController : ControllerBase
//    {
//        [HttpPost("v1/products")]
//        [Authorize]

//        public async Task<IActionResult> PostAsync(
//             [FromBody] RegisterProductViewModel model,
//             [FromServices] DataContext context)
//        {

//            try
//            {
//                if (!ModelState.IsValid)
//                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//                var user = await context
//                        .Users
//                        .AsNoTracking()
//                        .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

//                var product = new Product
//                {
//                    Name = model.Name,
//                    Description = model.Description,
//                    UserId = user.Id
//                };

//                await context.Products.AddAsync(product);
//                await context.SaveChangesAsync();

//                return Ok(new ResultViewModel<Product>(product));
//            }
//            catch
//            {
//                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
//            }
//        }

//        [HttpGet("v1/products")]
//        [Authorize]
//        public async Task<IActionResult> GetAsync(
//           [FromServices] DataContext context)
//        {
//            try
//            {
//                var user = await context
//                        .Users
//                        .AsNoTracking()
//                        .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

//                var products = await context.Products.Where(x => x.UserId == user.Id).ToListAsync();
//                return Ok(new ResultViewModel<List<Product>>(products));
//            }
//            catch
//            {
//                return StatusCode(500, new ResultViewModel<List<Product>>("05X04 - Falha interna no servidor"));
//            }
//        }

//        [HttpGet("v1/products/{id}")]
//        [Authorize]
//        public async Task<IActionResult> GetByIdAsync(
//           [FromRoute] int id,
//           [FromServices] DataContext context)
//        {
//            try
//            {
//                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);

//                if (product == null)
//                    return NotFound(new ResultViewModel<Product>("Produto não encontrado"));

//                return Ok(new ResultViewModel<Product>(product));
//            }
//            catch
//            {
//                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
//            }
//        }

//        [HttpPut("v1/products/{id}")]
//        [Authorize]
//        public async Task<IActionResult> PutAsync(
//            [FromRoute] int id,
//            [FromBody] Product model,
//            [FromServices] DataContext context)
//        {

//            try
//            {
//                if (!ModelState.IsValid)
//                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);

//                if (product == null)
//                    return NotFound(new ResultViewModel<Product>("Produto não encontrado"));

//                product.Name = model.Name;
//                product.Description = model.Description;

//                context.Products.Update(product);
//                await context.SaveChangesAsync();

//                return Ok(new ResultViewModel<string>("Produto editado com sucesso."));
//            }
//            catch
//            {
//                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
//            }
//        }


//        [HttpDelete("v1/products/{id}")]
//        [Authorize]
//        public async Task<IActionResult> DeleteAsync(
//            [FromRoute] int id,
//            [FromServices] DataContext context)
//        {
//            try
//            {
//                var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);

//                if (product == null)
//                    return NotFound(new ResultViewModel<Product>("Produto não encontrado"));

//                context.Products.Remove(product);
//                await context.SaveChangesAsync();

//                return Ok(new ResultViewModel<string>("Produto removido com sucesso"));
//            }
//            catch
//            {
//                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
//            }
//        }
//    }
//}
