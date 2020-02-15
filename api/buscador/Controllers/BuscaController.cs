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

        public BuscaController(IScraper scraper)
        {            
            _scraper = scraper;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetTeste()
        {
            try
            {
                 await _scraper.ExtrairDadosSites();
                return Ok("Dados de roupas exportadasl");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

    }
}