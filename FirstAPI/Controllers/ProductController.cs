using FirstAPI.Extensions;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Authorize]

    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
            => _productRepository = productRepository;

        [HttpPost("/products/register")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        [ProducesResponseType(typeof(ErrorViewModel), 500)]

        public async Task<IActionResult> RegisterProductAsync([FromBody] ProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = ModelState.GetErrors() });


                var product = await _productRepository.AddProductAsync(model, User.GetUserId());

                var result = new Product
                {
                    Id = product.Id,
                    Name = model.Name,
                    Description = model.Description,
                    UserId = User.GetUserId()
                };

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { error = "Falha interna no servidor" });
            }
        }

        [HttpGet("/products")]
        [ProducesResponseType(typeof(ProductsResult), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        [ProducesResponseType(typeof(ErrorViewModel), 404)]
        [ProducesResponseType(typeof(ErrorViewModel), 500)]

        public async Task<IActionResult> GetAllProductsAsync(
                   [FromQuery] int page = 0,
                   [FromQuery] int pageSize = 25)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = ModelState.GetErrors() });

                var count = await _productRepository.GetCountProductsByUserId(User.GetUserId());

                if (count == 0)
                    return NotFound(new { error = "Nenhum produto encontrado." });

                var products = await _productRepository.GetAllProductsByUserId(page * pageSize, pageSize, User.GetUserId());

                var result = new ProductsResult
                {
                    Total = count,
                    Page = page,
                    PageSize = pageSize,
                    Products = products
                };

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new { error = "Falha interna no servidor" });
            }
        }

        [HttpGet("/products/{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        [ProducesResponseType(typeof(ErrorViewModel), 404)]
        [ProducesResponseType(typeof(ErrorViewModel), 500)]

        public async Task<IActionResult> GetProductAsync([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = ModelState.GetErrors() });

                var product = await _productRepository.GetProductById(id);

                if (product == null)
                    return NotFound(new { error = "Produto não encontrado." });

                return Ok(product);
            }
            catch
            {
                return StatusCode(500, new { error = "Falha interna no servidor" });
            }
        }

        [HttpPut("/products/{id}")]
        [ProducesResponseType(typeof(MessageViewModel), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        [ProducesResponseType(typeof(ErrorViewModel), 401)]
        [ProducesResponseType(typeof(ErrorViewModel), 404)]
        [ProducesResponseType(typeof(ErrorViewModel), 500)]

        public async Task<IActionResult> UpdateProductAsync(
            [FromRoute] int id,
            [FromBody] ProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = ModelState.GetErrors() });

                var product = await _productRepository.GetProductById(id);

                if (product == null)
                    return NotFound(new { error = "Produto não encontrado." });

                if (product.UserId != User.GetUserId())
                    return Unauthorized(new { error = "Você não está autorizato para editar os dados deste produto." });

                await _productRepository.UpdateProductByIdAsync(model, id);

                return Ok(new { Message = "Dados editados com sucesso!" });
            }
            catch
            {
                return StatusCode(500, new { error = "Falha interna no servidor" });
            }
        }

        [HttpDelete("/products/{id}")]
        [ProducesResponseType(typeof(MessageViewModel), 200)]
        [ProducesResponseType(typeof(ErrorViewModel), 400)]
        [ProducesResponseType(typeof(ErrorViewModel), 401)]
        [ProducesResponseType(typeof(ErrorViewModel), 404)]
        [ProducesResponseType(typeof(ErrorViewModel), 500)]

        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { error = ModelState.GetErrors() });

                var product = await _productRepository.GetProductById(id);

                if (product == null)
                    return NotFound(new { error = "Produto não encontrado." });

                if (product.UserId != User.GetUserId())
                    return Unauthorized(new { error = "Você não está autorizato para remover este produto." });

                await _productRepository.DeleteProductByIdAsync(id);

                return Ok(new { Message = "Produto removido com sucesso." });
            }
            catch
            {
                return StatusCode(500, new { error = "Falha interna no servidor" });
            }
        }

    }
}









