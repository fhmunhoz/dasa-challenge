using buscador.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Collections.Generic;
using buscador.Models;
using buscador.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IO;
using System;
using AngleSharp.Dom;
using System.Text.RegularExpressions;
using System.Web;

namespace buscador.Services
{
    public class ScraperSiteVkModas : IScraperSite
    {
        private TemplateBusca _template { get; set; }
        private IBrowsingContext _browsingContext { get; set; }
        private readonly ILogger _logger;
        private readonly IBusca _roupa;

        public ScraperSiteVkModas(ILogger<ScraperSitePostHaus> logger,
        IBusca roupa)
        {
            _logger = logger;
            _roupa = roupa;
        }

        private decimal TratamentoPreco(string precoTexto)
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

        private bool PossuiMaisPaginas(IDocument paginaCategoria)
        {

            var botoesPaginacao = paginaCategoria.QuerySelectorAll(_template.SeletorBotaoProximaPagina);

            //Verifica se foi encontrado ao meno um botão de proxima página
            return botoesPaginacao.Length > 0;
        }

        private string UrlProximaPagina(string urlPaginaAtual)
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
                var indiceProximaPagina = Convert.ToInt32(qs.GetValues("page")[0]) + 1;
                qs.Set("page", indiceProximaPagina.ToString());

                urlProximaPagina = string.Format("{0}?{1}", urlPaginaAtual.Remove(qsIndex), qs.ToString());
            }
            else
            {
                urlProximaPagina = string.Format("{0}?page=2", urlPaginaAtual);
            }

            return urlProximaPagina;

        }

        private async Task ExtrairDadosPorCategoria(string urlGridCategoria,
                                                    string nomeCategoria,
                                                    List<ResultadoBusca> resultados)
        {

            var paginaCategoria = await _browsingContext.OpenAsync(urlGridCategoria);
            var produtos = paginaCategoria.QuerySelectorAll(_template.SeletorGridProdutos);

            foreach (var produtoGrid in produtos)
            {

                var eleLinkProduto = produtoGrid.QuerySelector(_template.SeletorLinkProduto);
                var urlProduto = eleLinkProduto.GetAttribute("value");

                try
                {

                    var paginaProduto = await _browsingContext.OpenAsync(urlProduto);
                    var nomeProduto = paginaProduto.QuerySelector(_template.SeletorNome).GetAttribute("content");

                    var roupa = new ResultadoBusca();
                    roupa.Origem = _template.Nome;
                    roupa.UrlProduto = urlProduto;
                    roupa.Nome = nomeProduto;
                    roupa.Descricao = paginaProduto.QuerySelector(_template.SeletorDescricao).GetAttribute("content");
                    roupa.Categoria = nomeCategoria;
                    roupa.UrlImagem = paginaProduto.QuerySelector(_template.SeletorUrlImagem).GetAttribute("content");
                    roupa.Preco = TratamentoPreco(paginaProduto.QuerySelector(_template.SeletorPreco).InnerHtml);

                    roupa.Tamanhos = new List<string>();
                    var elesTamanho = paginaProduto.QuerySelectorAll(_template.SeletorTamanhos);
                    foreach (var tamanho in elesTamanho)
                    {
                        roupa.Tamanhos.Add(tamanho.InnerHtml);
                    }

                    resultados.Add(roupa);
                    _logger.LogInformation("Produto {nomeProduto} da categoria {nomeCategoria} extraido do página: {urlProduto}",
                        nomeProduto, nomeCategoria, urlProduto);

                }
                catch (System.Exception ex)
                {
                    var msgErro = ex.Message;
                    _logger.LogError("Produto da página {urlProduto} e categoria {nomeCategoria} não foi extraido. Erro: {msgErro}",
                        urlProduto, nomeCategoria, msgErro);
                }

            }

            if (PossuiMaisPaginas(paginaCategoria))
            {
                var urlProximaPagina = UrlProximaPagina(urlGridCategoria);
                await ExtrairDadosPorCategoria(urlProximaPagina, nomeCategoria, resultados);
            }

        }

        private async Task ExtrairDadosPagina(List<ResultadoBusca> resultados)
        {

            // Load default configuration
            var config = Configuration.Default.WithDefaultLoader();
            // Create a new browsing context
            _browsingContext = BrowsingContext.New(config);

            //Encontra cada link de produto na tela principal de busca
            var gridProdutos = await _browsingContext.OpenAsync(_template.UrlInicial);

            //Encontras as TAGS com a url para a o grid de produtos por categoria
            var categorias = gridProdutos.QuerySelectorAll(_template.SeletorMenuCategorias);

            var buscaId = 0;
            foreach (var categoria in categorias)
            {

                var urlGridCategoria = categoria.GetAttribute("href");

                //Categoria está registrada no menu de grid de produtos, não foi encontrada 
                //dentro da página de detalhes do produto, 
                //padrão semelhante foi visto nos outros 3 sites
                //var nomeCategoria = categoria.QuerySelector(_template.SelectorCategoria).InnerHtml;
                var nomeCategoria = categoria.InnerHtml;

                //_logger.LogInformation("{nomeCategoria} - {InnerHtml}", urlGridCategoria, nomeCategoria);
                
                await ExtrairDadosPorCategoria(urlGridCategoria, nomeCategoria, resultados);
                buscaId = await _roupa.PersistirBusca(buscaId, resultados);
                resultados.Clear();
                
            }

        }

        public async Task<IEnumerable<ResultadoBusca>> ExtraiDadosPagina(TemplateBusca template)
        {
            _template = template;
            List<ResultadoBusca> resultado = new List<ResultadoBusca>();
            await ExtrairDadosPagina(resultado);
            return resultado;
        }


    }

}