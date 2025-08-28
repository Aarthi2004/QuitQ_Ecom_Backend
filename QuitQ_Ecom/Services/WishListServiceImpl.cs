using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Service
{
    public class WishlistServiceImpl : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistServiceImpl> _logger;

        public WishlistServiceImpl(IWishlistRepository wishlistRepository, IMapper mapper, ILogger<WishlistServiceImpl> logger)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<WishListDTO>> GetUserWishList(int userId)
        {
            try
            {
                var userWishList = await _wishlistRepository.GetUserWishList(userId);
                return _mapper.Map<List<WishListDTO>>(userWishList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user's wishlist: {Message}", ex.Message);
                throw new GetUserWishlistException("Failed to retrieve user's wishlist", ex);
            }
        }

        public async Task<WishListDTO> AddToWishList(WishListDTO wishListDTO)
        {
            try
            {
                var wishList = _mapper.Map<WishList>(wishListDTO);
                var addedWishList = await _wishlistRepository.AddToWishList(wishList);
                return _mapper.Map<WishListDTO>(addedWishList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding to wishlist: {Message}", ex.Message);
                throw new AddToWishlistException("Failed to add item to wishlist", ex);
            }
        }

        public async Task<bool> RemoveFromWishList(int userId, int productId)
        {
            try
            {
                return await _wishlistRepository.RemoveFromWishList(userId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing from wishlist: {Message}", ex.Message);
                throw new RemoveFromWishlistException("Failed to remove item from wishlist", ex);
            }
        }
    }
}