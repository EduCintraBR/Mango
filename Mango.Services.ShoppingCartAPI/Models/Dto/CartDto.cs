namespace Mango.Services.ShoppingCartAPI.Models.Dto
{
    public class CartDto
    {
        public CartDetailsDto CartHeader { get; set; }
        public IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
