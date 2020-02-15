namespace buscador.Interfaces
{

    public interface IScraperFactory
    {
        IScraperSite RetornaScraperPorNome(string nome);
    }
}