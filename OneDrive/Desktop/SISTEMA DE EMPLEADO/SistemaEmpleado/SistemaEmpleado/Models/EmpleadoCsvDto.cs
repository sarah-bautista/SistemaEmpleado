namespace SistemaEmpleado.Models
{
    public class EmpleadoCsvDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Departamento { get; set; }
        public string Cargo { get; set; }
        public string FechaInicio { get; set; }
        public string Salario { get; set; }
        public string AFP { get; set; }
        public string ARS { get; set; }
        public string ISR { get; set; }
        public string TiempoEnEmpresa { get; set; }
        public string Estado { get; set; }
    }
}
