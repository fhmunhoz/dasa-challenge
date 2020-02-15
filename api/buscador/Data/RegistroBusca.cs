using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace buscador.Data
{
    [Table("RegistroBusca")]
    public class RegistroBusca
    {

        [Key]        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required()]        
        [StringLength(50)]
        [Column(TypeName = "text")]
        public string DataHora { get; set; }

        [Required()]        
        [StringLength(50)]
        [Column(TypeName = "text")]
        public string NomeSiteOrigem { get; set; }

        [ForeignKey("BuscaId")]
        public ICollection<Roupas> Roupas { get; set; }

    }

}