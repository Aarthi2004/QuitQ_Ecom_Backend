using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class OrderRepositoryImpl : IOrder
    {
        private readonly QuitQEcomContext _context;
        private readonly IMapper _mapper;
        private readonly IProduct _productRepo;
        private readonly IUserAddressRepository _userAddressRepo;
        private readonly IPayment _paymentRepo;
        private readonly IOrderItem _orderItemRepo;
        private readonly ICart _cartRepo;
        private readonly ILogger<OrderRepositoryImpl> _logger;

        public OrderRepositoryImpl(QuitQEcomContext quitQEcomContext, IMapper mapper, IProduct product, IUserAddressRepository userAddress, IPayment paymentRepo, IOrderItem orderItem, ICart cart, ILogger<OrderRepositoryImpl> logger)
        {
            _context = quitQEcomContext;
            _mapper = mapper;
            _productRepo = product;
            _userAddressRepo = userAddress;
            _paymentRepo = paymentRepo;
            _orderItemRepo = orderItem;
            _cartRepo = cart;
            _logger = logger;
        }

        private List<CartDTO> GetCartItemsByUserId(int userId)
        {
            var cartitems = _context.Carts.Where(x => x.UserId == userId).ToList();
            return _mapper.Map<List<CartDTO>>(cartitems);
        }

        public async Task<OrderDTO> CreateOrder(int userId, List<CartDTO> cartitemsList, string shippingAddress)
        {
            try
            {
                decimal totalAmount = 0.0M;
                foreach (var cartitem in cartitemsList)
                {
                    var productObj = _context.Products.FirstOrDefault(x => x.ProductId == cartitem.ProductId);
                    totalAmount += cartitem.Quantity * productObj.Price;
                }

                var orderObj = new Order()
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.Now,
                    OrderStatus = "pending",
                    ShippingAddress = shippingAddress
                };

                await _context.Orders.AddAsync(orderObj);
                await _context.SaveChangesAsync();

                var shipperObj = new Shipper() { OrderId = orderObj.OrderId };
                _context.Shippers.Add(shipperObj);
                await _context.SaveChangesAsync();

                return _mapper.Map<OrderDTO>(orderObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteOrderById(int orderId)
        {
            try
            {
                var orderObj = await _context.Orders.FindAsync(orderId);
                if (orderObj == null) return false;

                _context.Orders.Remove(orderObj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order with ID {OrderId}: {Message}", orderId, ex.Message);
                return false;
            }
        }

        public async Task<Dictionary<bool, string>> PlaceOrder(int userId, string paymentType)
        {
            var result = new Dictionary<bool, string>();
            var cartItems = GetCartItemsByUserId(userId);
            var productsStatus = await _productRepo.CheckQuantityOfProducts(cartItems);
            if (productsStatus.Count() > 0)
            {
                result[false] = "Could not place order. Some products are out of stock.";
                return result;
            }

            var userShippingAddress = await _userAddressRepo.GetActiveUserAddressByUserId(userId);
            if (userShippingAddress == null)
            {
                result[false] = "Could not place order. Please select the delivery address.";
                return result;
            }

            string shippingAddress = userShippingAddress.ToString();

            try
            {
                var orderDtoObj = await CreateOrder(userId, cartItems, shippingAddress);
                if (orderDtoObj == null)
                {
                    result[false] = "Could not place the order. Try again later.";
                    return result;
                }

                PaymentDTO paymentDtoObj = await _paymentRepo.AddNewPayment(orderDtoObj, paymentType);
                if (paymentDtoObj == null)
                {
                    await DeleteOrderById(orderDtoObj.OrderId);
                    result[false] = "Could not process payment. If amount debited will be refunded. Could not place the order.";
                    return result;
                }

                await _productRepo.UpdateQuantitiesOfProducts(cartItems);
                await _orderItemRepo.AddNewOrderItem(cartItems, orderDtoObj);
                await _cartRepo.RemoveCartItemsOfUser(userId);

                result[true] = "Successfully placed order.";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to place order for user ID {UserId}.", userId);
                throw;
            }
        }

        public async Task<List<OrderDTO>> ViewAllOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Shipper)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var orderDTOs = orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                ShippingAddress = o.ShippingAddress,
                Shipper = o.Shipper?.ShipperName,
                orderItemListDTOs = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    OrderItemId = oi.OrderItemId,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Product = oi.Product != null ? new ProductDTO
                    {
                        ProductId = oi.Product.ProductId,
                        ProductName = oi.Product.ProductName,
                        ProductImage = oi.Product.ProductImage,
                    } : new ProductDTO
                    {
                        ProductId = -1,
                        ProductName = "Product Not Available",
                    }
                }).ToList()
            }).ToList();

            return orderDTOs;
        }

        public async Task<OrderDTO> ViewOrderByOrderId(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null) return null;

                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {orderId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<OrderDTO>> ViewOrdersByStoreId(int storeId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Where(o => o.OrderItems != null && o.OrderItems.Any(oi => oi.Product != null && oi.Product.StoreId == storeId))
                    .ToListAsync();

                var orderDTOs = orders.Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    OrderStatus = order.OrderStatus,
                    ShippingAddress = order.ShippingAddress,
                    orderItemListDTOs = order.OrderItems
                        .Where(oi => oi.Product != null && oi.Product.StoreId == storeId)
                        .Select(oi => new OrderItemDTO
                        {
                            OrderItemId = oi.OrderItemId,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            Product = oi.Product != null ? new ProductDTO
                            {
                                ProductId = oi.Product.ProductId,
                                ProductName = oi.Product.ProductName,
                                ProductImage = oi.Product.ProductImage,
                            } : new ProductDTO
                            {
                                ProductId = -1,
                                ProductName = "Product Not Available",
                            }
                        }).ToList()
                }).ToList();

                return orderDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for seller ID {storeId}: {ex.Message}");
                throw;
            }
        }

        public async Task<List<OrderDTO>> ViewAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Shipper)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .ToListAsync();

                var orderDTOs = orders.Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    ShippingAddress = o.ShippingAddress,
                    Shipper = o.Shipper?.ShipperName,
                    orderItemListDTOs = o.OrderItems.Select(oi => new OrderItemDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Product = oi.Product != null ? new ProductDTO
                        {
                            ProductId = oi.Product.ProductId,
                            ProductName = oi.Product.ProductName,
                            ProductImage = oi.Product.ProductImage,
                        } : new ProductDTO
                        {
                            ProductId = -1,
                            ProductName = "Product Not Available",
                        }
                    }).ToList()
                }).ToList();

                return orderDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all orders: {Message}", ex.Message);
                return new List<OrderDTO>();
            }
        }
    }
}