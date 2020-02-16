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
        private readonly IOptions<List<TemplateBusca>> _templates;
        //private readonly IScraperFactory _scraperRepository;     
        private readonly IScraperSite _scraperDistritoModas;   
        private readonly IBusca _roupa;
          private readonly ILogger _logger;
        public Scraper(ScraperDbContext context,                        
                        IOptions<List<TemplateBusca>> templates,                        
                        IBusca roupa,
                        IScraperFactory scraperRepository,
                        IScraperSite scraperPostHaus,
                        ILogger<Scraper> logger
                        )
        {
            _context = context;            
            _templates = templates;            
            _roupa = roupa;
            _logger = logger;
            //_scraperRepository = scraperRepository;            

            _scraperDistritoModas = scraperPostHaus;
        }

        public async Task ExtrairDadosSites()
        {
                        
            foreach (var busca in _templates.Value)
            {

                if (string.IsNullOrEmpty(busca.UrlInicial))
                {
                    //Se o seletor principal estiver em branco, não será possivel fazer a leitura
                    continue;
                }

                try{
                                        
                    await _scraperDistritoModas.ProcessaDadosPagina(busca);
                }
                catch(System.Exception ex){

                    var nomeSite = busca.Nome;
                    var msgErro = ex.Message;
                    _logger.LogError("Ocorreu um erro ao importar os dados do site {nomeSite}. Erro: {msgErro}",nomeSite, msgErro);                    

                }
                
                
            }

        }

    }


}
