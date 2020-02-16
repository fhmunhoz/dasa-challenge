using buscador.Interfaces;
using System.Text.RegularExpressions;
using System;
using System.Web;

namespace buscador.Services
{

    public class ScraperHelper : IScraperHelper
    {

        public decimal TratamentoPreco(string precoTexto)
        {

            //Utilizar regex para remover todos os caractres que não forem numeros ou ,
            //preco = preco.Replace(',', '.');
            var padraoPreco = @"[0-9\,\.]+";
            var encontrouPreco = Regex.IsMatch(precoTexto, padraoPreco);

            if (!encontrouPreco)
                throw new IndexOutOfRangeException("Preço do produto não foi encontrado");

            var precoFormatado = Regex.Match(precoTexto, padraoPreco).Value;
            var preco = decimal.Parse(precoFormatado);
            return preco;

        }

        public string UrlProximaPagina(string urlPaginaAtual, string queryStringId)
        {

            //página atual possui QS pag, se não 
            //incluir a query ?pag=2
            //se possui, descobre o valor da página
            var urlProximaPagina = "";

            var qsIndex = urlPaginaAtual.IndexOf("?");
            if (qsIndex > 0)
            {
                var querystring = urlPaginaAtual.Substring(qsIndex + 1);
                var qs = HttpUtility.ParseQueryString(querystring);
                var indiceProximaPagina = Convert.ToInt32(qs.GetValues(queryStringId)[0]) + 1;
                qs.Set(queryStringId, indiceProximaPagina.ToString());

                urlProximaPagina = string.Format("{0}?{1}", urlPaginaAtual.Remove(qsIndex), qs.ToString());
            }
            else
            {
                urlProximaPagina = string.Format("{0}?{1}=2", urlPaginaAtual,queryStringId);
            }

            return urlProximaPagina;

        }


    }

}