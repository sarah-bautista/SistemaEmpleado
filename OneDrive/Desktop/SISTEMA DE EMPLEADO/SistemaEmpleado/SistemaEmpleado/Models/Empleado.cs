using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaEmpleado.Models
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un departamento.")]
        public int DepartamentoId { get; set; }

        [ValidateNever]
        public Departamento Departamento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cargo.")]
        public int CargoId { get; set; }

        [ValidateNever]
        public Cargo Cargo { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha de inicio.")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar el salario.")]
        [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser positivo.")]
        public decimal Salario { get; set; }

        public bool Estado { get; set; }

        // Propiedades calculadas
        [NotMapped]
        public decimal AFP => Math.Round(Salario * 0.0287M, 2);

        [NotMapped]
        public decimal ARS => Math.Round(Salario * 0.0304M, 2);

        [NotMapped]
        public decimal ISR
        {
            get
            {
                if (Salario <= 34685) return 0;
                else if (Salario <= 52027) return (Salario - 34685) * 0.15M;
                else if (Salario <= 72260) return (Salario - 52027) * 0.20M + 2601;
                else return (Salario - 72260) * 0.25M + 6648;
            }
        }

        [NotMapped]
        public decimal SueldoNeto => Salario - AFP - ARS - ISR;

        [NotMapped]
        public string TiempoEnEmpresa
        {
            get
            {
                var hoy = DateTime.Today;
                var años = hoy.Year - FechaInicio.Year;
                var meses = hoy.Month - FechaInicio.Month;
                if (meses < 0)
                {
                    años--;
                    meses += 12;
                }
                return $"{años} años, {meses} meses";
            }
        }
    }
}