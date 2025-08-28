using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repositories;
using QuitQ_Ecom.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Service
{
    public class CartServiceImpl : ICartService
    {
        private readonly ICart _cartRepo;
        private readonly IProduct _productRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CartServiceImpl> _logger;

        public CartServiceImpl(ICart cartRepo, IProduct productRepo, IMapper mapper, ILogger<CartServiceImpl> logger)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CartDTO>> GetUserCartItems(int userId)
        {
            try
            {
                var cartItems = await _cartRepo.GetUserCartItems(userId);

                if (cartItems == null || !cartItems.Any())
                {
                    return new List<CartDTO>();
                }

                var productIds = cartItems.Select(c => c.ProductId).Distinct().ToList();
                var products = await _productRepo.GetProductsByIds(productIds);

                if (products == null || !products.Any())
                {
                    _logger.LogWarning($"No products found for user ID {userId} in cart.");
                    return _mapper.Map<List<CartDTO>>(cartItems);
                }

                var cartItemsDto = _mapper.Map<List<CartDTO>>(cartItems);

                foreach (var cartItemDto in cartItemsDto)
                {
                    var product = products.FirstOrDefault(p => p.ProductId == cartItemDto.ProductId);
                    if (product != null)
                    {
                        cartItemDto.ProductPrice = product.Price;
                        cartItemDto.ProductName = product.ProductName;
                        cartItemDto.ProductImage = product.ProductImage;
                    }
                }

                return cartItemsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user cart items.");
                throw new GetUserCartItemsException("Failed to get user cart items.", ex);
            }
        }

        public async Task<CartDTO> AddProductToCart(CartDTO cartItemDTO, int userId)
        {
            _logger.LogInformation($"Attempting to add product ID: {cartItemDTO.ProductId} for user ID: {userId}.");

            try
            {
                if (cartItemDTO.Quantity <= 0)
                {
                    throw new InvalidCartItemException("Quantity should be greater than zero.");
                }

                var existingCartItem = await _cartRepo.GetCartItemByProductIdAndUserId(userId, cartItemDTO.ProductId);

                if (existingCartItem != null)
                {
                    _logger.LogInformation($"Found existing cart item ID: {existingCartItem.CartId} for product ID: {existingCartItem.ProductId}.");

                    var product = await _productRepo.GetProductById(cartItemDTO.ProductId);
                    if (product == null)
                    {
                        throw new ProductNotFoundException($"Product with ID {cartItemDTO.ProductId} not found.");
                    }
                    if (product.Quantity < existingCartItem.Quantity + cartItemDTO.Quantity)
                    {
                        throw new InsufficientStockException($"Insufficient stock for product {product.ProductName}.");
                    }
                    existingCartItem.Quantity += cartItemDTO.Quantity;
                    await _cartRepo.UpdateCartItem(existingCartItem);
                    return _mapper.Map<CartDTO>(existingCartItem);
                }
                else
                {
                    _logger.LogInformation($"No existing cart item found. Adding a new one for product ID: {cartItemDTO.ProductId}.");

                    var product = await _productRepo.GetProductById(cartItemDTO.ProductId);
                    if (product == null)
                    {
                        throw new ProductNotFoundException($"Product with ID {cartItemDTO.ProductId} not found.");
                    }
                    if (product.Quantity < cartItemDTO.Quantity)
                    {
                        throw new InsufficientStockException($"Insufficient stock for product {product.ProductName}.");
                    }

                    var cart = _mapper.Map<Cart>(cartItemDTO);
                    cart.UserId = userId;
                    var addedCartItem = await _cartRepo.AddNewProductToCart(cart);
                    return _mapper.Map<CartDTO>(addedCartItem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add new product to cart.");
                throw;
            }
        }

        public async Task<bool> IncreaseProductQuantity(int cartItemId, int userId)
        {
            try
            {
                var cartItem = await _cartRepo.GetCartItemById(cartItemId);
                if (cartItem == null || cartItem.UserId != userId)
                {
                    _logger.LogWarning($"Attempt to increase quantity for non-existent or unauthorized cart item ID: {cartItemId} by user ID: {userId}.");
                    return false;
                }

                var product = await _productRepo.GetProductById(cartItem.ProductId);
                if (product == null)
                {
                    throw new ProductNotFoundException($"Product with ID {cartItem.ProductId} not found.");
                }

                if (product.Quantity < cartItem.Quantity + 1)
                {
                    throw new InsufficientStockException($"Insufficient stock for product {product.ProductName}.");
                }

                return await _cartRepo.IncreaseProductQuantity(cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to increase product quantity in cart.");
                throw;
            }
        }

        public async Task<bool> DecreaseProductQuantity(int cartItemId, int userId)
        {
            try
            {
                var cartItem = await _cartRepo.GetCartItemById(cartItemId);
                if (cartItem == null || cartItem.UserId != userId)
                {
                    _logger.LogWarning($"Attempt to decrease quantity for non-existent or unauthorized cart item ID: {cartItemId} by user ID: {userId}.");
                    return false;
                }

                if (cartItem.Quantity - 1 <= 0)
                {
                    return await _cartRepo.RemoveProductFromCart(cartItemId);
                }

                return await _cartRepo.DecreaseProductQuantity(cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrease product quantity in cart.");
                throw;
            }
        }

        public async Task<bool> RemoveProductFromCart(int cartItemId, int userId)
        {
            try
            {
                var cartItem = await _cartRepo.GetCartItemById(cartItemId);
                if (cartItem == null || cartItem.UserId != userId)
                {
                    _logger.LogWarning($"Attempt to remove non-existent or unauthorized cart item ID: {cartItemId} by user ID: {userId}.");
                    return false;
                }

                return await _cartRepo.RemoveProductFromCart(cartItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove product from cart.");
                throw;
            }
        }

        public async Task<decimal> GetCartTotalCost(int userId)
        {
            try
            {
                var cartItems = await GetUserCartItems(userId);
                return cartItems.Sum(item => item.ProductPrice.GetValueOrDefault(0) * item.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate total cart cost.");
                throw;
            }
        }

        public async Task<bool> RemoveCartItemsOfUser(int userId)
        {
            try
            {
                return await _cartRepo.RemoveCartItemsOfUser(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove all cart items for user.");
                throw;
            }
        }
    }
}