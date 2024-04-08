using ProdutoCatalogo.Application.DTOs;

namespace ProdutoCatalogo.Application.Helpers;

public class PaginationService
{
    public class PaginationResponse<T>
    {
        public bool ExisteMaisPaginas { get; set; }
        public int PaginaAtual { get; set; }
        public int TotalDePaginas { get; set; }
        public int TotalDeItens { get; set; }
        public int ItensDaLista { get; set; }
        public int LimiteDeItens { get; set; }
    }

    public static PaginationResponse<T> CreatePaginationResponse<T>(IEnumerable<T> items, int totalItems, Pagination pagination)
    {
        int totalPages = (int)Math.Ceiling((double)totalItems / pagination.tamanho);
        int itemsInList = items.ToList().Count;
        int itemsLimit = pagination.tamanho;
        int currentPage = pagination.pagina;
        bool hasMorePages = (totalPages - currentPage) > 0;

        var response = new PaginationResponse<T>
        {
            ExisteMaisPaginas = hasMorePages,
            PaginaAtual = currentPage,
            TotalDePaginas = totalPages,
            TotalDeItens = totalItems,
            ItensDaLista = itemsInList,
            LimiteDeItens = itemsLimit
        };

        return response;
    }
}
