
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public Guid UsuarioId { get; set; }

        [Required(ErrorMessage ="O nome é obrigatório")]
        [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} e no mínimo {2} caracteres",
            MinimumLength = 5)]
        public string Nome { get; set; }

        [Required]
        public string Email { get; set; }

        public Guid? LoginId { get; set; }

        public Perfil Perfil { get; set; }
        public int? PerfilId { get; set; }

    }
}
