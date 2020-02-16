using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace buscador.Helpers
{
    public class PagingHelper<T> : List<T>
    {
        public int PaginaAtual { get; private set; }
        public int TotalPaginas { get; private set; }

        public PagingHelper(List<T> items, int totalItens, int paginaAtual, int itensPorPagina)
        {
            PaginaAtual = paginaAtual;
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)itensPorPagina);

            this.AddRange(items);
        }

        public bool PossuiPaginaAnterior
        {
            get
            {
                return (PaginaAtual > 1);
            }
        }

        public bool PossuiProximaPagina
        {
            get
            {
                return (PaginaAtual < TotalPaginas);
            }
        }

        public static async Task<PagingHelper<T>> CriarPaginacao(IQueryable<T> source, int paginaAtual, int itensPorPagina)
        {
            var totalItens = await source.CountAsync();
            var items = await source.Skip((paginaAtual - 1) * itensPorPagina).Take(itensPorPagina).ToListAsync();
            return new PagingHelper<T>(items, totalItens, paginaAtual, itensPorPagina);
        }
    }
}