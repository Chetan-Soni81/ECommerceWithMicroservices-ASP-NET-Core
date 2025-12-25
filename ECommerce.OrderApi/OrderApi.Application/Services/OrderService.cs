using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient, 
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        // GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call Product API using HttpClient
            // Redirect this call to the API Gateway since the Product API is not responds to outsider.
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        // GET USER
        public async Task<AppUserDTO> GetUser(int clientId)
        {
            // Call User API using HttpClient
            // Redirect this call to the API Gateway since the User API is not responds to outsider.
            var getUser = await httpClient.GetAsync($"/api/products/{clientId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order.Id <=0)
                return null!;

            // Get Retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            // Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare User
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // Populate OrderDetailsDTO
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                order.PurchaseQuantity * productDTO.Price,
                order.OrderedDate
                );
        }

        // GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all Client's orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any()) return null!;

            // Convert entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
