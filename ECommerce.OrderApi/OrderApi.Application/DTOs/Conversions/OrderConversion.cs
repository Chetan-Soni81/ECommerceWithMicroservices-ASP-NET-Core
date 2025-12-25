using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO order) =>  new()
        {
            Id= order.Id,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderedDate = order.OrderedDate,
            PurchaseQuantity = order.PurchaseQuantity
        };

        public static (OrderDTO? , IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            // return single order DTO
            if (order is not null || orders is null)
            {
                var singleDTO = new OrderDTO(
                    order!.Id,
                    order!.ProductId,
                    order!.ClientId,
                    order!.PurchaseQuantity,
                    order!.OrderedDate);
                return (singleDTO, null);
            }

            // return multiple order DTOs
            if (orders is not null || order is null)
            {
                var _orders = orders!.Select(o => new OrderDTO(
                    o.Id, 
                    o.ProductId, 
                    o.ClientId, 
                    o.PurchaseQuantity, 
                    o.OrderedDate)).ToList();
                return (null, _orders);
            }

            return (null, null);
        }
    }
}
