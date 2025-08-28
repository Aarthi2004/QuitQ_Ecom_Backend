using AutoMapper;
using Microsoft.Extensions.Logging;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class GenderService : IGenderService
    {
        private readonly IGenderRepository _genderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GenderService> _logger;

        public GenderService(IGenderRepository genderRepository, IMapper mapper, ILogger<GenderService> logger)
        {
            _genderRepository = genderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GenderDTO> AddGender(GenderDTO genderDTO)
        {
            var gender = _mapper.Map<Gender>(genderDTO);
            var addedGender = await _genderRepository.AddGender(gender);
            return _mapper.Map<GenderDTO>(addedGender);
        }

        public async Task<bool> DeleteGender(int genderId)
        {
            return await _genderRepository.DeleteGender(genderId);
        }

        public async Task<List<GenderDTO>> GetAllGenders()
        {
            var genders = await _genderRepository.GetAllGenders();
            return _mapper.Map<List<GenderDTO>>(genders);
        }

        public async Task<GenderDTO> GetGenderById(int genderId)
        {
            var gender = await _genderRepository.GetGenderById(genderId);
            return _mapper.Map<GenderDTO>(gender);
        }

        public async Task<GenderDTO> UpdateGender(GenderDTO genderDTO)
        {
            var gender = _mapper.Map<Gender>(genderDTO);
            var updatedGender = await _genderRepository.UpdateGender(gender);
            return _mapper.Map<GenderDTO>(updatedGender);
        }
    }
}
