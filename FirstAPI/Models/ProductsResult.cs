namespace FirstAPI.Models
{
    public class ProductsResult
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public List<Product> Products { get; set; }
    }
}
