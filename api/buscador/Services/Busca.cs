using System.Collections.Generic;
using buscador.Interfaces;
using buscador.Models;
using buscador.Data;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace buscador.Services
{
    public class Busca : IBusca
    {

        private readonly ScraperDbContext _context;
        private readonly ILogger _logger;

        public Busca(ScraperDbContext context,
                        ILogger<Busca> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> PersistirBusca(int buscaId, IEnumerable<ResultadoBusca> resultados)
        {
            try
            {

                foreach (var resultado in resultados)
                {

                    if (buscaId == 0)
                    {
                        //Gera o ID de busca, para agrupar as consultas em datas diferentes
                        RegistroBusca registro = new RegistroBusca
                        {
                            DataHora = DateTime.Now.ToString("dd/MM/yyyy hh:mm"),
                            NomeSiteOrigem = resultado.Origem

                        };

                        _context.RegistroBusca.Add(registro);
                        _context.SaveChanges();
                        buscaId = registro.Id;
                    }

                    var roupa = new Roupas
                    {
                        BuscaId = buscaId,
                        Categoria = resultado.Categoria,
                        Descricao = resultado.Descricao,
                        Nome = resultado.Nome,
                        Preco = resultado.Preco,
                        UrlImagem = resultado.UrlImagem,
                        UrlProduto = resultado.UrlProduto
                    };

                    roupa.Tamanhos = new List<RoupasTamanho>();
                    resultado.Tamanhos.ForEach(t =>
                        roupa.Tamanhos.Add(new RoupasTamanho
                        {
                            Tamanho = t
                        })
                    );

                    await _context.Roupas.AddAsync(roupa);

                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Resultado persistido");

                return buscaId;

            }
            catch (System.Exception ex)
            {
                var msgErro = ex.Message;
                _logger.LogError("Erro ao persitir resultado. Erro: {msgErro}", msgErro);
                throw;
            }

        }

        public async Task ConsolidarBusca(int id)
        {
            var busca = await _context.RegistroBusca.FindAsync(id);

            try
            {
                var resultadosBusca = from b in _context.Roupas where b.BuscaId == id select b;

                foreach (var item in resultadosBusca)
                {
                    //Será usado como chave para consolidar a busca
                    var urlProduto = item.UrlProduto;

                    var produtoConsolidado = (from p
                                                in _context.BuscaConsolidada
                                              where p.UrlProduto == urlProduto
                                              select p).FirstOrDefault();

                    //Tamanho deverá ser atualizado em ambos os casos de novo ou produto já existente
                    var tamanhos = string.Join(',', (from tam
                                                        in _context.RoupasTamanho
                                                     where tam.RoupaId == item.Id
                                                     select tam.Tamanho).ToArray());

                    //Caso for um novo produto,
                    //adiciona novamente a lista de consolidado com a flag novo produto
                    if (produtoConsolidado == null)
                    {
                        produtoConsolidado = new BuscaConsolidada
                        {
                            Categoria = item.Categoria,
                            Descricao = item.Descricao,
                            MaiorPreco = false,
                            MenorPreco = false,
                            Nome = item.Nome,
                            Origem = busca.NomeSiteOrigem,
                            Preco = item.Preco,
                            ProdutoNovo = true,
                            UrlImagem = item.UrlImagem,
                            UrlProduto = item.UrlProduto,
                            Tamanhos = tamanhos
                        };

                        await _context.BuscaConsolidada.AddAsync(produtoConsolidado);

                    }
                    else
                    {

                        //Encontro o valor maximo e minimo do mesmo produto nas outras buscas                    
                        var precosMesmoProduto = (from vMax
                                            in _context.Roupas
                                                  where vMax.UrlProduto == item.UrlProduto && vMax.BuscaId != busca.Id
                                                  select vMax.Preco).ToList();

                        decimal valorMinimo = 0;
                        decimal valorMaximo = 0;

                        if (precosMesmoProduto.Any())
                        {
                            valorMinimo = precosMesmoProduto.Min();
                            valorMaximo = precosMesmoProduto.Max();
                        }

                        //Caso não retorno resultados, a flag será false para o menor ou maior valor
                        produtoConsolidado.MenorPreco = valorMinimo > 0 ? produtoConsolidado.Preco < valorMinimo : false;
                        produtoConsolidado.MaiorPreco = valorMaximo > 0 ? produtoConsolidado.Preco > valorMaximo : false;
                        produtoConsolidado.ProdutoNovo = false;
                        produtoConsolidado.Tamanhos = tamanhos;
                        produtoConsolidado.Nome = item.Nome;
                        produtoConsolidado.Descricao = item.Nome;

                    }

                }

                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                var msgErro = ex.Message;
                var origem = busca.NomeSiteOrigem;

                _logger.LogError("Erro ao consolidar a busca {origem}. Erro: {msgErro}", msgErro);
                throw new Exception("Ocorreu um erro ao consolidar a busca.");
                throw;
            }

        }

        public async Task<Helpers.PagingHelper<BuscaViewModel>> CompararProdutos(string termoBusca, int paginaAtual, int itensPagina)
        {

            try
            {
                var resultados = (from bus in _context.BuscaConsolidada
                                  where bus.Categoria.ToLower().Contains(termoBusca.ToLower())
                                  select new BuscaViewModel
                                  {
                                      Categoria = bus.Categoria,
                                      Descricao = bus.Descricao,
                                      Nome = bus.Nome,
                                      Origem = bus.Origem,
                                      Preco = bus.Preco,
                                      Tamanhos = bus.Tamanhos,
                                      UrlImagem = bus.UrlImagem,
                                      UrlProduto = bus.UrlProduto,
                                      PrecoOrdenacao = bus.PrecoOrdenacao
                                  }).OrderByDescending(o => o.PrecoOrdenacao);

                var resultadosPaginados = await Helpers.PagingHelper<BuscaViewModel>.CriarPaginacao(resultados, paginaAtual, itensPagina);
                return resultadosPaginados;

            }
            catch (System.Exception ex)
            {
                var msgErro = ex.Message;
                _logger.LogError("Ocorreu um erro ao retornar os itens. Erro: {msgErro}", msgErro);
                throw new Exception("Ocorreu um erro ao retornar os itens.");
            }

        }

    }
}