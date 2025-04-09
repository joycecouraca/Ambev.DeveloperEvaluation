namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.Common.Response
{
    public class ProductResponse : ProductResponseBase
    { 
        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Gets or sets the quantity of the product.
        /// </summary>
        public int Quantity { get; set; }
    }
}
