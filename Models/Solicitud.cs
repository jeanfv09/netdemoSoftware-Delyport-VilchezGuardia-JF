using System;
using System.ComponentModel.DataAnnotations;

namespace SISDELYPORT.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [Display(Name = "Nombre del Cliente")]
        public string NombreCliente { get; set; } = "";

        [Required(ErrorMessage = "El distrito de destino es obligatorio")]
        [Display(Name = "Distrito de Destino")]
        public string DistritoDestino { get; set; } = "";

        [Required(ErrorMessage = "La descripción del paquete es obligatoria")]
        [Display(Name = "Descripción del Paquete")]
        public string DescripcionPaquete { get; set; } = "";

        [Required(ErrorMessage = "La fecha requerida es obligatoria")]
        [Display(Name = "Fecha Requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaRequerida { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public string CodigoSolicitud { get; set; } = "";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string HistorialCambios { get; set; } = "";

        public string MotivoRechazo { get; set; } = "";

    }
}

