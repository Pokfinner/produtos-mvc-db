using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdutosMVC.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProdutosMVC.Controllers;
[Route("product")]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var produtos = await _context.Products.ToListAsync();
        return View(produtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();
        return View(product);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return StatusCode(400, "Product not Valid.");
    }

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // Ensure this method handles POST requests correctly
    [HttpPost("delete/{id}"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("api")]
    public async Task<IActionResult> TodosProdutos()
    {
        var produtos = await _context.Products.ToListAsync();
        return Json(produtos);
    }

    [HttpGet("api/em-stock/{status}")]
    public async Task<IActionResult> ProdutosPorStock(bool status)
    {
        var filteredProducts = await _context.Products.Where(p => p.InStock == status).ToListAsync();
        return Json(filteredProducts);
    }

    [HttpGet("api/acima-preco/{minPrice}")]
    public async Task<IActionResult> ProdutosAcimaPreco(double minPrice)
    {
        var filteredProducts = await _context.Products.Where(p => p.Price >= minPrice).ToListAsync();
        return Json(filteredProducts);
    }

    [HttpGet("api/{id}")]
    public async Task<IActionResult> ProdutoIndividual(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();
        return Json(product);
    }

    [HttpPost("api")]
    public async Task<IActionResult> AdicionarProduto([FromBody] Product produto)
    {
        if (produto == null)
            return BadRequest();

        _context.Products.Add(produto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ProdutoIndividual), new { id = produto.Id }, produto);
    }

    [HttpPut("api/{id}")]
    public async Task<IActionResult> AlterarProduto(int id, [FromBody] Product updatedProduct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();
        if (updatedProduct == null)
            return BadRequest();

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        product.Description = updatedProduct.Description;
        product.InStock = updatedProduct.InStock;

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    [HttpDelete("api/{id}")]
    public async Task<IActionResult> EliminarProduto(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok("Produto eliminado!");
    }
}