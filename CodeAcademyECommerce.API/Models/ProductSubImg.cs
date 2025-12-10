using Microsoft.EntityFrameworkCore;

namespace CodeAcademyECommerce.API
{
    //[PrimaryKey(nameof(SubImg), nameof(ProductId))]
    public class ProductSubImg
    {
        public int Id { get; set; }
        public string SubImg { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}
