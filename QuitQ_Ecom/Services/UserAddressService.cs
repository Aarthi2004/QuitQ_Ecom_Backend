using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IUser _userRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAddressService> _logger;

        public UserAddressService(
            IUserAddressRepository userAddressRepository,
            IUser userRepository,
            ICityRepository cityRepository,
            IMapper mapper,
            ILogger<UserAddressService> logger)
        {
            _userAddressRepository = userAddressRepository;
            _userRepository = userRepository;
            _cityRepository = cityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserAddressDTO> AddUserAddress(UserAddressDTO userAddressDTO)
        {
            try
            {
                if (userAddressDTO.UserId.HasValue)
                {
                    // CORRECTED: Call the existing method on your interface
                    var userExists = await _userRepository.GetUserByIdAsync(userAddressDTO.UserId.Value);
                    if (userExists == null)
                    {
                        throw new UserAddressAddException("User not found.");
                    }
                }
                else
                {
                    throw new UserAddressAddException("User ID is required.");
                }

                // You also need to verify that CityId exists, as discussed.
                var cityExists = await _cityRepository.GetCityById(userAddressDTO.CityId);
                if (cityExists == null)
                {
                    throw new UserAddressAddException("City not found.");
                }

                var userAddress = _mapper.Map<UserAddress>(userAddressDTO);
                var addedUserAddress = await _userAddressRepository.AddUserAddress(userAddress);
                return _mapper.Map<UserAddressDTO>(addedUserAddress);
            }
            catch (UserAddressAddException ex)
            {
                _logger.LogError(ex, "Failed to add user address due to validation error: {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding a user address.");
                throw new UserAddressAddException("An unexpected error occurred.", ex);
            }
        }

        public async Task<UserAddressDTO> UpdateUserAddress(int userAddressId, UserAddressDTO userAddressDTO)
        {
            try
            {
                // This method also needs a check for CityId, similar to AddUserAddress
                var cityExists = await _cityRepository.GetCityById(userAddressDTO.CityId);
                if (cityExists == null)
                {
                    throw new UserAddressUpdateException("City not found.");
                }

                var userAddress = _mapper.Map<UserAddress>(userAddressDTO);
                var updatedUserAddress = await _userAddressRepository.UpdateUserAddress(userAddressId, userAddress);

                if (updatedUserAddress == null)
                {
                    throw new UserAddressNotFoundException($"User address with ID {userAddressId} not found.");
                }
                return _mapper.Map<UserAddressDTO>(updatedUserAddress);
            }
            catch (UserAddressNotFoundException)
            {
                throw;
            }
            catch (UserAddressUpdateException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user address with ID {UserAddressId}.", userAddressId);
                throw new UserAddressUpdateException("An unexpected error occurred while updating the user address.", ex);
            }
        }

        public async Task<UserAddressDTO> GetActiveUserAddressByUserId(int userId)
        {
            var userAddress = await _userAddressRepository.GetActiveUserAddressByUserId(userId);
            if (userAddress == null)
            {
                throw new UserAddressNotFoundException($"Active user address not found for user ID {userId}");
            }
            return _mapper.Map<UserAddressDTO>(userAddress);
        }

        public async Task<bool> DeleteUserAddress(int userAddressId)
        {
            var deleted = await _userAddressRepository.DeleteUserAddress(userAddressId);
            if (!deleted)
            {
                throw new UserAddressNotFoundException($"User address with ID {userAddressId} not found.");
            }
            return true;
        }

        public async Task<List<UserAddressDTO>> GetUserAddressesByUserId(int userId)
        {
            var userAddresses = await _userAddressRepository.GetUserAddressesByUserId(userId);

            if (userAddresses == null || userAddresses.Count == 0)
            {
                throw new UserAddressNotFoundException($"No user addresses found for user ID {userId}.");
            }

            return _mapper.Map<List<UserAddressDTO>>(userAddresses);
        }
    }
}