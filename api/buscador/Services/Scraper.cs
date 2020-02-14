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

namespace buscador.Services
{

    public class Scraper : IScraper
    {

        private readonly ScraperDbContext _context;
        private Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IOptions<List<TemplateBusca>> _templates;

        public Scraper(ScraperDbContext context,
                        Microsoft.Extensions.Configuration.IConfiguration configuration,
                        IOptions<List<TemplateBusca>> templates
                        )
        {
            _context = context;
            _configuration = configuration;
            _templates = templates;
        }

        private decimal TratamentoPreco(string preco)
        {

            if (string.IsNullOrEmpty(preco))
            {
                return 0;
            }

            //Utilizar regex para remover todos os caractres que não forem numeros ou ,
            //preco = preco.Replace(',', '.');
            preco = preco.Replace("R$", "");
            preco = preco.Trim();
            return decimal.Parse(preco);
        }

        private async Task ExtrairDadosPagina(TemplateBusca template, List<ResultadoBusca> resultados)
        {

            // Load default configuration
            var config = Configuration.Default.WithDefaultLoader();
            // Create a new browsing context
            var context = BrowsingContext.New(config);

            //Encontra cada link de produto na tela principal de busca, ou páginadas
            var gridProdutos = await context.OpenAsync(template.UrlGrid);

            var produtos = gridProdutos.QuerySelectorAll(template.SeletorGridProdutos);

            foreach (var produtoGrid in produtos)
            {
                var eleLinkProduto = produtoGrid.QuerySelector(template.SeletorLinkProduto);

                var urlProduto = template.UrlSite + eleLinkProduto.GetAttribute("href");
                var paginaProduto = await context.OpenAsync(urlProduto);
                
                var roupa = new Roupas();
                roupa.DataBusca = DateTime.Now;
                roupa.Origem = template.Nome;
                roupa.UrlProduto = urlProduto;
                roupa.Nome = paginaProduto.QuerySelector(template.SeletorNome).InnerHtml;
                roupa.Descricao = paginaProduto.QuerySelector(template.SeletorDescricao).InnerHtml;
                roupa.Categoria = "Não achei";
                roupa.UrlImagem = "Não consegui";
                roupa.Preco = TratamentoPreco(paginaProduto.QuerySelector(template.SeletorPreco).InnerHtml);

                roupa.Tamanhos = new List<RoupasTamanho>();
                var elesTamanho = paginaProduto.QuerySelectorAll(template.SeletorTamanhos);

                foreach (var tamanho in elesTamanho)
                {
                    roupa.Tamanhos.Add(new RoupasTamanho
                    {
                        Tamanho = tamanho.InnerHtml
                    });
                }

                _context.Roupas.Add(roupa);

            }

            await _context.SaveChangesAsync();

            //TODO: Verificar se existem mais páginas, caso sim
            //Alterar a propriedade template.UrlGrid ára o link da página "paginada" e chama a função deleitura novamente

        }

        public async Task ExtrairDadosSites()
        {
            //var teste = _configuration.Get<string>("ConfiguracoesBuscador");
            //var urlsBusca = _configuration.GetSection("ConfiguracoesBuscador").GetValue<List<TemplateBusca>>("Sites");

            foreach (var busca in _templates.Value)
            {

                if (string.IsNullOrEmpty(busca.UrlGrid))
                {
                    //Se o seletor principal estiver em branco, não será possivel fazer a leitura
                    continue;
                }

                List<ResultadoBusca> resultados = new List<ResultadoBusca>();
                await ExtrairDadosPagina(busca, resultados);

                //TODO: Persistir resultados no banco de dados.

            }

        }

    }


}
