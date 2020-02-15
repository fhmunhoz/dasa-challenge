using buscador.Interfaces;
using System;

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
            return (IScraperSite)_serviceProvider.GetService(typeof(ScraperSitePostHaus)) ;
         }                    
         else if(name == "distritomoda") {
            return (IScraperSite)_serviceProvider.GetService(typeof(ScraperSiteDistritoModa)) ;
         } 
         else if(name == "vkmodas") {
            return (IScraperSite)_serviceProvider.GetService(typeof(ScraperSiteVkModas)) ;
         } 
         else{
             throw new NotImplementedException(string.Format("Serviço {0} não encontrado", name));
         }           
    }

}

}

