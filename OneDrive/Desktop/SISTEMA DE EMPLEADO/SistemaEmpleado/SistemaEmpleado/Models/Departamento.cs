using SistemaEmpleado.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaEmpleado.Models;
public class Departamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    public string Nombre { get; set; }

    public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
