using Microsoft.AspNetCore.Mvc;
using search_product_mvc.Models;
using search_product_mvc.Repositories;
using search_product_mvc.Services;

namespace search_product_mvc.Controllers;

public class UserController : Controller
{
    private readonly IRepository<User> _userRepository;
    private readonly ILuceneService<User> _luceneService;

    public UserController(IRepository<User> userRepository, ILuceneService<User> luceneService)
    {
        _userRepository = userRepository;
        _luceneService = luceneService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var users = await _userRepository.GetAllAsync();
        if (!string.IsNullOrEmpty(search))
        {
            users = _luceneService.Search(search, maxHits: 10).ToList();
        }
        return View(users);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }
        _userRepository.Create(user);
        await _userRepository.SaveAsync();
        _luceneService.Add(user);
        _luceneService.Commit();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }
        _userRepository.Update(user);
        await _userRepository.SaveAsync();
        _luceneService.Update(user);
        _luceneService.Commit();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        _userRepository.Delete(id);
        await _userRepository.SaveAsync();
        _luceneService.Delete(id);
        _luceneService.Commit();
        return RedirectToAction(nameof(Index));
    }

    public async Task<string> BuildIndex()
    {

        var products = await _userRepository.GetAllAsync();
        _luceneService.AddRange(products);
        _luceneService.Commit();

        return "Index built successfully!";
    }

    public async Task<string> RebuildIndex()
    {
        var products = await _userRepository.GetAllAsync();
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
}