
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Perfil")]
    public class Perfil
    {
        public Perfil ()
        {
            Permissao = new Collection<Permissao>();
            Usuario = new Collection<Usuario>();
        }
        [Key]
        public int PerfilId { get; set; }

        [Required(ErrorMessage ="O nome é obrigatório")]
        [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} e no mínimo {2} caracteres",
            MinimumLength = 5)]
        public string Descricao { get; set; }

        public ICollection<Permissao> Permissao { get; set; }
        public ICollection<Usuario> Usuario { get; set; }

    }
}
