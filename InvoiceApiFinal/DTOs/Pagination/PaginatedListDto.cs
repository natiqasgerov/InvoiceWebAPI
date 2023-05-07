namespace InvoiceApiFinal.DTOs.Pagination
{
    public class PaginatedListDto<TModel>
    {
        public IEnumerable<TModel> Items { get; }
        public PaginationMeta Meta { get; }
        public PaginatedListDto(IEnumerable<TModel> items, PaginationMeta meta)
        {
            Items = items;
            Meta = meta;
        }
    }
}
