namespace buscador.Interfaces
{
    public interface IScraperHelper
    {
        string UrlProximaPagina(string urlPaginaAtual, string QuerySTringId);
        decimal TratamentoPreco(string precoTexto);
    }
}

