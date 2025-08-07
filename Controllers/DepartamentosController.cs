using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEmpleados.Data;
using SistemaEmpleado.Models;

public class DepartamentosController : Controller
{
    private readonly AppDbContext _context;
    public DepartamentosController(AppDbContext context) => _context = context;

    // INDEX: Lista todos los departamentos
    public async Task<IActionResult> Index() =>
        View(await _context.Departamentos.ToListAsync());

    // DETAILS: Muestra los detalles de un departamento
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var dept = await _context.Departamentos
            .FirstOrDefaultAsync(m => m.Id == id);

        if (dept == null) return NotFound();

        return View(dept);
    }

    // CREATE (GET)
    public IActionResult Create() => View();

    // CREATE (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Departamento departamento)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(departamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        return View(departamento);
    }

    // EDIT (GET)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var dept = await _context.Departamentos.FindAsync(id);
        if (dept == null) return NotFound();
        return View(dept);
    }

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Departamento departamento)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Update(departamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        return View(departamento);
    }

    // DELETE (GET)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var dept = await _context.Departamentos.FirstOrDefaultAsync(m => m.Id == id);
        if (dept == null) return NotFound();
        return View(dept);
    }

    // DELETE (POST)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var departamento = await _context.Departamentos.FindAsync(id);
            _context.Departamentos.Remove(departamento);
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
