using System.Threading.Tasks;
using buscador.Models;
using System.Collections.Generic;
using AngleSharp.Dom;

namespace buscador.Interfaces
{

    public interface IRoupas
    {
        Task PersistirBusca(IEnumerable<ResultadoBusca> resultado);
    }
     
}