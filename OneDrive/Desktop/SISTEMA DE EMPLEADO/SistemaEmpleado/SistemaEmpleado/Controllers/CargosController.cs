using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEmpleados.Data;
using SistemaEmpleado.Models;

public class CargosController : Controller
{
    private readonly AppDbContext _context;
    public CargosController(AppDbContext context) => _context = context;

    // INDEX: Lista todos los cargos
    public async Task<IActionResult> Index() =>
        View(await _context.Cargos.ToListAsync());

    // DETAILS: Muestra detalles de un cargo
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var cargo = await _context.Cargos
            .FirstOrDefaultAsync(m => m.Id == id);

        if (cargo == null) return NotFound();

        return View(cargo);
    }

    // CREATE (GET)
    public IActionResult Create() => View();

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cargo cargo)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(cargo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        return View(cargo);
    }

    // EDIT (GET)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var cargo = await _context.Cargos.FindAsync(id);
        if (cargo == null) return NotFound();
        return View(cargo);
    }

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Cargo cargo)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Update(cargo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        return View(cargo);
    }

    // DELETE (GET)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var cargo = await _context.Cargos.FirstOrDefaultAsync(m => m.Id == id);
        if (cargo == null) return NotFound();
        return View(cargo);
    }

    // DELETE (POST)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var cargo = await _context.Cargos.FindAsync(id);
            _context.Cargos.Remove(cargo);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
        return RedirectToAction(nameof(Index));
    }
}
