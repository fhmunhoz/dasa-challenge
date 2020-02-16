using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using buscador.Interfaces;


namespace buscador.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuscaController : ControllerBase
    {
        private readonly IScraper _scraper;
        private readonly IBusca _busca;

        public BuscaController(IScraper scraper,
                                IBusca busca)
        {
            _scraper = scraper;
            _busca = busca;
        }

        [HttpGet("{termoBusca}/{paginaAtual:int}/{itensPorPagina:int}")]
        public async Task<ActionResult> Getbusca(string termoBusca, int paginaAtual, int itensPorPagina)
        {
            if (string.IsNullOrWhiteSpace(termoBusca))
                return BadRequest("Preencha algum termo de busca");
            if (itensPorPagina <= 0)
                return BadRequest("Itens por página deve ser maior do que zero");
            if (paginaAtual <= 0)
                return BadRequest("Pagina atual deve ser maior do que zero");

            try
            {
                var retorno = await _busca.CompararProdutos(termoBusca, paginaAtual, itensPorPagina);

                return Ok(new
                {
                    resultados = retorno,
                    PaginaAtual = retorno.PaginaAtual,
                    TotalPaginas = retorno.TotalPaginas,
                    PossuiPaginaAnterior = retorno.PossuiPaginaAnterior,
                    PossuiProximaPagina = retorno.PossuiProximaPagina
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

    }
}