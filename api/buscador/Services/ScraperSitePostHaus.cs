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

    public class ScraperSitePostHaus : IScraperSite
    {
        private TemplateBusca _template { get; set; }
        private IBrowsingContext _browsingContext { get; set; }
        
        private readonly ILogger _logger;
        private readonly IBusca _roupa;
        private readonly IScraperHelper _helper;

        public ScraperSitePostHaus(ILogger<ScraperSitePostHaus> logger,
                                        IBusca roupa,
                                        IScraperHelper helper)
        {
            _logger = logger;
            _roupa = roupa;
            _helper = helper;
        }
   
        private bool PossuiMaisPaginas(IDocument paginaCategoria)
        {

            var botoesPaginacao = paginaCategoria.QuerySelectorAll(_template.SeletorBotaoProximaPagina);
            IElement botaoProximaPagina = null;

            if(botoesPaginacao.Length == 0) return false;

            //Se o SPAN do botão de paginação for encontrado e seu texto contem
            //proximo, existe mais páginas
            //Caso contrário não existe
            //a esolha é feita pelo innertext e posição do botão 
            //pois o seletor dos botões de página anterior e próxima são iguais
            //quando os dois botões estão habilitados, a diferença encontrada foi o texto
            if (botoesPaginacao.Length > 1)
            {
                botaoProximaPagina = botoesPaginacao[1];
            }
            else
            {
                botaoProximaPagina = botoesPaginacao[0];
            }

            if (botaoProximaPagina != null && botaoProximaPagina.InnerHtml.ToLower().Contains("&gt;"))
            {
                return true;
            }

            return false;

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
                var urlProduto = _template.UrlSite + eleLinkProduto.GetAttribute("href");

                try
                {

                    var paginaProduto = await _browsingContext.OpenAsync(urlProduto);
                    var nomeProduto = paginaProduto.QuerySelector(_template.SeletorNome).InnerHtml.Trim();

                    var roupa = new ResultadoBusca();
                    roupa.Origem = _template.Nome;
                    roupa.UrlProduto = urlProduto;
                    roupa.Nome = nomeProduto;
                    roupa.Descricao = paginaProduto.QuerySelector(_template.SeletorDescricao).GetAttribute("content");
                    roupa.Categoria = nomeCategoria;
                    roupa.UrlImagem = paginaProduto.QuerySelector(_template.SeletorUrlImagem).GetAttribute("content");
                    roupa.Preco = _helper.TratamentoPreco(paginaProduto.QuerySelector(_template.SeletorPreco).InnerHtml.Trim());

                    roupa.Tamanhos = new List<string>();
                    var elesTamanho = paginaProduto.QuerySelectorAll(_template.SeletorTamanhos);
                    foreach (var tamanho in elesTamanho)
                    {
                        roupa.Tamanhos.Add(tamanho.InnerHtml.Trim());
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
                var urlProximaPagina = _helper.UrlProximaPagina(urlGridCategoria, _template.QueryStringPaginacao);
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

                var urlGridCategoria = _template.UrlSite + categoria.GetAttribute("href");

                //Categoria está registrada no menu de grid de produtos, não foi encontrada 
                //dentro da página de detalhes do produto, 
                //padrão semelhante foi visto nos outros 3 sites
                var nomeCategoria = categoria.QuerySelector(_template.SelectorCategoria).InnerHtml.Trim();
                
                await ExtrairDadosPorCategoria(urlGridCategoria, nomeCategoria, resultados);
                buscaId = await _roupa.PersistirBusca(buscaId, resultados);
                resultados.Clear();
            }

        }

        public async Task ProcessaDadosPagina(TemplateBusca template)
        {
            _template = template;
            List<ResultadoBusca> resultado = new List<ResultadoBusca>();
            await ExtrairDadosPagina(resultado);            
        }

    }

}