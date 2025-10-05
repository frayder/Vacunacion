using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    [Table("HistorialCargaPacientes")]
    public class HistorialCargaPacientes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaCarga { get; set; } = DateTime.Now;

        [Required]
        public string Usuario { get; set; } = string.Empty;

        public int TotalCargados { get; set; }

        public int TotalExistentes { get; set; }

        public string? ArchivoNombre { get; set; }

        public string? Observaciones { get; set; }
    }
}
