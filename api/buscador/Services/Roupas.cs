using System.Collections.Generic;
using buscador.Interfaces;
using buscador.Models;
using buscador.Data;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace buscador.Services
{
    public class Roupa : IRoupas
    {

        private readonly ScraperDbContext _context; 
        private readonly ILogger _logger; 

        public Roupa(ScraperDbContext context,
                        ILogger<Roupa> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task PersistirBusca(IEnumerable<ResultadoBusca> resultados)
        {
            try
            {
                
                foreach (var resultado in resultados)
                {
                    
                    var roupa = new Roupas{
                        Categoria = resultado.Categoria,
                        DataBusca = DateTime.Now,
                        Descricao = resultado.Descricao,
                        Nome = resultado.Nome,
                        Origem = resultado.Origem,
                        Preco = resultado.Preco,
                        UrlImagem = resultado.UrlImagem,
                        UrlProduto = resultado.UrlProduto
                    };

                    roupa.Tamanhos = new List<RoupasTamanho>();
                    resultado.Tamanhos.ForEach(t => 
                        roupa.Tamanhos.Add(new RoupasTamanho{
                            Tamanho = t
                        })
                    );

                   await _context.Roupas.AddAsync(roupa);

                }
                
                await _context.SaveChangesAsync();
                _logger.LogInformation("Resultado persistido");

            }
            catch (System.Exception ex)
            {
                var msgErro = ex.Message;
                _logger.LogError("Erro ao persitir resultado. Erro: {msgErro}",msgErro);
                throw;
            }

            

        }

    }
}