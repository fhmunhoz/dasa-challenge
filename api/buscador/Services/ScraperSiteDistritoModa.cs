using System.Collections.Generic;
using buscador.Interfaces;
using buscador.Models;
using System.Threading.Tasks;

namespace buscador.Services
{
    public class ScraperSiteDistritoModa : IScraperSite
    {
        public ScraperSiteDistritoModa()
        {

        }

        public async Task<IEnumerable<ResultadoBusca>> ExtraiDadosPagina(TemplateBusca template)
        {
            return null;
        }

    }
}