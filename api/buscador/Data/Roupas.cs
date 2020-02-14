using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace buscador.Data
{
    [Table("Roupas")]
    public class Roupas
    {

        [Key]        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required()]
        [StringLength(255)]
        [Column(TypeName = "text")]
        public string Nome { get; set; }

        [Required()]
        [StringLength(255)]
        [Column(TypeName = "text")]
        public string Descricao { get; set; }

        [Required()]        
        [Column(TypeName = "real")]
        public decimal Preco { get; set; }

        [Required()]        
        [StringLength(255)]
        [Column(TypeName = "text")]
        public string UrlProduto { get; set; }

        [Required()]        
        [StringLength(255)]
        [Column(TypeName = "text")]
        public string UrlImagem { get; set; }

        [Required()]        
        [StringLength(50)]
        [Column(TypeName = "text")]
        public string Categoria { get; set; }
        
        [Required()]        
        [StringLength(25)]
        [Column(TypeName = "text")]
        public string Origem { get; set; }

        [Required()]     
        [Column(TypeName = "text")]   
        [DataType(DataType.Date)]
        public DateTime DataBusca {get;set;}

        [ForeignKey("RoupasTamanhoFK")]
        public ICollection<RoupasTamanho> Tamanhos { get; set; }

    }

}