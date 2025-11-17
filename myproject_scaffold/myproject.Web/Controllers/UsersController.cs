using Microsoft.AspNetCore.Mvc;
using myproject.Service;

namespace myproject.Web.Controllers;

public class UsersController(IUserService service) : Controller
{
    public async Task<IActionResult> Index(string? email, bool? isActive, CancellationToken ct)
    {
        var users = await service.SearchAsync(email, isActive, ct);
        return View(users);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(string email, string displayName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(displayName))
        {
            ModelState.AddModelError("", "Email and Display Name are required.");
            return View();
        }
        try
        {
            await service.CreateAsync(email, displayName, ct);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    [HttpGet]
    public IActionResult Edit(Guid id) => View(new EditUserVm { Id = id });

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserVm vm, CancellationToken ct)
    {
        var updated = await service.UpdateAsync(vm.Id, vm.DisplayName, vm.IsActive, ct);
        if (updated is null) return NotFound();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }
}

public class EditUserVm
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
