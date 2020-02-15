using System.Collections.Generic;
using buscador.Interfaces;
using buscador.Models;
using buscador.Data;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

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

    }
}