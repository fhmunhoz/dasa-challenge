using System.Threading.Tasks;
using buscador.Models;
using System.Collections.Generic;
using AngleSharp.Dom;

namespace buscador.Interfaces
{

    public interface IScraperSitePosthaus : IScraperSite
    {
    }


    public interface IScraperSiteVKModas : IScraperSite
    {
    }

    public interface IScraperSiteDistritoModas : IScraperSite
    {
    }

    public interface IScraperSite
    {
        Task ProcessaDadosPagina(TemplateBusca template);
    }

}

