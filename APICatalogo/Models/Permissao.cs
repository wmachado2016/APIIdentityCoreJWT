using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Permissao")]
    public class Permissao
    {
      
        [Key]
        public int PermissaoId { get; set; }

        [Required(ErrorMessage ="O nome é obrigatório")]
        [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} e no mínimo {2} caracteres",
            MinimumLength = 5)]
        public string Descricao { get; set; }
    }
}
