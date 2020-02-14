using System.Collections.Generic;

namespace buscador.Models
{    
    public class ResultadoBusca
    {         
        public string Origem { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }        
        public decimal UrlProduto { get; set; }        
        public decimal UrlImagem { get; set; }        
        public decimal Categoria { get; set; }
        public IEnumerable<string> Tamanhos {get;set;}

    }

}