using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Exceptions;
using QuitQ_Ecom.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class GenderRepository : IGenderRepository
    {
        private readonly QuitQEcomContext _context;
        private readonly ILogger<GenderRepository> _logger;

        public GenderRepository(QuitQEcomContext quitQEcomContext, ILogger<GenderRepository> logger)
        {
            _context = quitQEcomContext;
            _logger = logger;
        }

        public async Task<Gender> AddGender(Gender gender)
        {
            try
            {
                await _context.Genders.AddAsync(gender);
                await _context.SaveChangesAsync();
                return gender;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding gender.");
                throw new AddGenderException("Failed to add gender.", ex);
            }
        }

        public async Task<bool> DeleteGender(int genderId)
        {
            try
            {
                var gender = await _context.Genders.FindAsync(genderId);
                if (gender == null)
                    return false;

                _context.Genders.Remove(gender);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting gender with ID {GenderId}", genderId);
                throw new DeleteGenderException("Failed to delete gender.", ex);
            }
        }

        public async Task<List<Gender>> GetAllGenders()
        {
            try
            {
                return await _context.Genders.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving genders.");
                throw new GetAllGendersException("Failed to retrieve all genders.", ex);
            }
        }

        public async Task<Gender> GetGenderById(int genderId)
        {
            try
            {
                var gender = await _context.Genders.FindAsync(genderId);
                if (gender == null)
                    throw new GenderNotFoundException($"Gender with ID {genderId} not found.");
                return gender;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gender with ID {GenderId}", genderId);
                throw new GetGenderByIdException("Failed to retrieve gender by ID.", ex);
            }
        }

        public async Task<Gender> UpdateGender(Gender gender)
        {
            try
            {
                _context.Genders.Update(gender);
                await _context.SaveChangesAsync();
                return gender;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating gender.");
                throw new UpdateGenderException("Failed to update gender.", ex);
            }
        }
    }
}
