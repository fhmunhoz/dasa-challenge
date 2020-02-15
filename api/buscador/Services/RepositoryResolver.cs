using buscador.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace buscador.Services
{

    public class ScraperFactory : IScraperFactory 
{
    private readonly IServiceProvider _serviceProvider;
    public ScraperFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IScraperSite RetornaScraperPorNome(string name)
    {
         if(name == "posthaus") {
            return _serviceProvider.GetService<ScraperSitePostHaus>();
         }                    
         else if(name == "distritomoda") {
            return _serviceProvider.GetService<ScraperSiteDistritoModa>() ;
         } 
         else if(name == "vkmodas") {
            return _serviceProvider.GetService<ScraperSiteVkModas>() ;
         } 
         else{
             throw new NotImplementedException(string.Format("Serviço {0} não encontrado", name));
         }           
    }

}

}

