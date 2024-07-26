using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using search_product_mvc.Data;
using search_product_mvc.Models;
using search_product_mvc.Services;

namespace search_product_mvc.Controllers;


public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILuceneService<Product> _luceneService;

    public ProductController(AppDbContext context, ILuceneService<Product> luceneService)
    {
        _context = context;
        _luceneService = luceneService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var products = await _context.Products.ToListAsync();
        if (!string.IsNullOrEmpty(search))
        {
            products = _luceneService.Search(search, maxHits: 10).ToList();
        }
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    public string BuildIndex()
    {

        var products = _context.Products.ToList();
        _luceneService.AddRange(products);
        _luceneService.Commit();

        return "Index built successfully!";
    }

    public string RebuildIndex()
    {
        var products = _context.Products.ToList();
        _luceneService.Clear();
        _luceneService.AddRange(products);
        _luceneService.Commit();
        return "Index rebuilt successfully!";
    }

    public string ClearIndex()
    {
        _luceneService.Clear();
        _luceneService.Commit();
        return "Index cleared successfully!";
    }

    public IActionResult Edit()
    {
        return View();
    }
}