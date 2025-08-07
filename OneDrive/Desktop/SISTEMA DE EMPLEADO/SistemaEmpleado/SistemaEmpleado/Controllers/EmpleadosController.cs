using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaEmpleados.Data;
using SistemaEmpleado.Models;
using System.Globalization;
using System.Text;
using CsvHelper;

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
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
            }
        }
        else
        {
            ModelState.AddModelError("", "Por favor, complete todos los campos correctamente.");
        }

        ViewBag.DepartamentoId = new SelectList(_context.Departamentos, "Id", "Nombre", empleado.DepartamentoId);
        ViewBag.CargoId = new SelectList(_context.Cargos, "Id", "Nombre", empleado.CargoId);
        return View(empleado);
    }

    public async Task<IActionResult> Editar(int id)
    {
        var empleado = await _context.Empleados.FindAsync(id);
        if (empleado == null) return NotFound();
        ViewBag.DepartamentoId = new SelectList(_context.Departamentos, "Id", "Nombre", empleado.DepartamentoId);
        ViewBag.CargoId = new SelectList(_context.Cargos, "Id", "Nombre", empleado.CargoId);
        return View(empleado);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Empleado empleado)
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

    [HttpPost]
    public async Task<IActionResult> ExportCsv()
    {
        var empleados = await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Cargo)
            .ToListAsync();

        var empleadosCsv = empleados.Select(e => new EmpleadoCsvDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Departamento = e.Departamento?.Nombre ?? "Sin departamento",
            Cargo = e.Cargo?.Nombre ?? "Sin cargo",
            FechaInicio = e.FechaInicio.ToString("dd/MM/yyyy"),
            Salario = e.Salario.ToString("C", new CultureInfo("es-DO")),
            AFP = e.AFP.ToString("C", new CultureInfo("es-DO")),
            ARS = e.ARS.ToString("C", new CultureInfo("es-DO")),
            ISR = e.ISR.ToString("C", new CultureInfo("es-DO")),
            TiempoEnEmpresa = CalcularTiempo(e.FechaInicio),
            Estado = e.Estado ? "Activo" : "Inactivo"
        }).ToList();

        using var mem = new MemoryStream();
        using var writer = new StreamWriter(mem, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(empleadosCsv);
        writer.Flush();
        mem.Position = 0;

        return File(mem.ToArray(), "text/csv", $"empleados_{DateTime.Now:yyyyMMdd}.csv");
    }

    private string CalcularTiempo(DateTime fechaInicio)
    {
        var hoy = DateTime.Today;
        var años = hoy.Year - fechaInicio.Year;
        var meses = hoy.Month - fechaInicio.Month;

        if (meses < 0)
        {
            años--;
            meses += 12;
        }

        return $"{años} años, {meses} meses";
    }
}
