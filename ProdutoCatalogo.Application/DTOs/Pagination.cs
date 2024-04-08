using ProdutoCatalogo.Application.Validators;
using ProdutoCatalogo.Shared.Configurations;
using ProdutoCatalogo.Shared.Messages;
using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Application.DTOs
{
    public class Pagination
    {
        [Range(PaginationValues.MinPageNumber, int.MaxValue, ErrorMessage = ValidationMessages.Pagination.Current)]
        public int pagina { get; set; } = PaginationValues.MinPageNumber;

        [RangeValues(10, 50, 100, 150, ErrorMessage = ValidationMessages.Pagination.Size)]
        public int tamanho { get; set; } = PaginationValues.DefaultPageSize;
    }
}
