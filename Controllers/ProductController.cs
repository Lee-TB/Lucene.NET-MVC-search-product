using Microsoft.AspNetCore.Mvc;
using search_product_mvc.Models;
using search_product_mvc.Repositories;
using search_product_mvc.Services;

namespace search_product_mvc.Controllers;


public class ProductController : Controller
{
    private readonly IRepository<Product> _productRepository;
    private readonly ILuceneService<Product> _luceneService;

    public ProductController(IRepository<Product> productRepo, ILuceneService<Product> luceneService)
    {
        _productRepository = productRepo;
        _luceneService = luceneService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var products = await _productRepository.GetAllAsync();
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
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            product.Id = Guid.NewGuid();
            _productRepository.Create(product);
            await _productRepository.SaveAsync();
            _luceneService.Add(product);
            _luceneService.Commit();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            _productRepository.Update(product);
            await _productRepository.SaveAsync();
            _luceneService.Update(product);
            _luceneService.Commit();
            return RedirectToAction("Index");
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        System.Console.WriteLine("Delete");
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        _productRepository.Delete(product);
        await _productRepository.SaveAsync();
        _luceneService.Delete(product);
        _luceneService.Commit();
        return RedirectToAction("Index");
    }


    public async Task<string> BuildIndex()
    {

        var products = await _productRepository.GetAllAsync();
        _luceneService.AddRange(products);
        _luceneService.Commit();

        return "Index built successfully!";
    }

    public async Task<string> RebuildIndex()
    {
        var products = await _productRepository.GetAllAsync();
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