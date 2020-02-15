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
        //private readonly IRepositoryResolver _scraperRepository;     
        private readonly IScraperSite _scraperPostHaus;   
        private readonly IRoupas _roupa;
        public Scraper(ScraperDbContext context,                        
                        IOptions<List<TemplateBusca>> templates,
                        IScraperSite scraperPostHaus,
                        IRoupas roupa
                        )
        {
            _context = context;            
            _templates = templates;
            _scraperPostHaus = scraperPostHaus;
            _roupa = roupa;
            //_scraperRepository = scraperRepository;            
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

                //var scrapService = _scraperRepository.GetRepositoryByName(busca.Nome);
                IEnumerable<ResultadoBusca> resultados = new List<ResultadoBusca>();                
                resultados = await _scraperPostHaus.ExtraiDadosPagina(busca);
                
            }

        }

    }


}
