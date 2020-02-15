   using System.Collections.Generic;
   using buscador.Interfaces;
   using buscador.Models;
   using System.Threading.Tasks;
   
   namespace buscador.Services
{
   public class ScraperSiteVkModas : IScraperSite
    {
        public ScraperSiteVkModas()
        {

        }

        public async Task<IEnumerable<ResultadoBusca>> ExtraiDadosPagina(TemplateBusca template)
        {       
            return null;
        }

    }

}