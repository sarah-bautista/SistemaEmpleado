using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEmpleados.Data;
using SistemaEmpleado.Models;
using System.Globalization;

public class EmpleadosController : Controller
{
    private readonly AppDbContext _context;
    public EmpleadosController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var empleados = await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Cargo)
            .ToListAsync();
        return View(empleados);
    }

    public IActionResult Create()
    {
        ViewBag.DepartamentoId = new SelectList(_context.Departamentos, "Id", "Nombre");
        ViewBag.CargoId = new SelectList(_context.Cargos, "Id", "Nombre");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Empleado empleado)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        ViewBag.DepartamentoId = new SelectList(_context.Departamentos, "Id", "Nombre", empleado.DepartamentoId);
        ViewBag.CargoId = new SelectList(_context.Cargos, "Id", "Nombre", empleado.CargoId);
        return View(empleado);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var empleado = await _context.Empleados.FindAsync(id);
        if (empleado == null) return NotFound();
        ViewBag.DepartamentoId = new SelectList(_context.Departamentos, "Id", "Nombre", empleado.DepartamentoId);
        ViewBag.CargoId = new SelectList(_context.Cargos, "Id", "Nombre", empleado.CargoId);
        return View(empleado);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Empleado empleado)
    {
        if (id != empleado.Id) return NotFound();
        try
        {
            if (ModelState.IsValid)
            {
                _context.Update(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        ViewBag.DepartamentoId = new SelectList(_context.Departamentos, "Id", "Nombre", empleado.DepartamentoId);
        ViewBag.CargoId = new SelectList(_context.Cargos, "Id", "Nombre", empleado.CargoId);
        return View(empleado);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var empleado = await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Cargo)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (empleado == null) return NotFound();
        return View(empleado);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var empleado = await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Cargo)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (empleado == null) return NotFound();
        return View(empleado);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var empleado = await _context.Empleados.FindAsync(id);
            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    // Acción Exportar CSV
    public async Task<IActionResult> ExportCsv()
    {
        var empleados = await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Cargo)
            .ToListAsync();

        using var mem = new MemoryStream();
        using var writer = new StreamWriter(mem);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(empleados);
        writer.Flush();

        return File(mem.ToArray(), "text/csv", "empleados.csv");
    }
}
