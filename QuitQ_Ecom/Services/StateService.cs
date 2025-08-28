using AutoMapper;
using QuitQ_Ecom.DTOs;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using QuitQ_Ecom.Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Services
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public StateService(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _mapper = mapper;
        }

        public async Task<StateDTO> AddState(StateDTO stateDTO)
        {
            if (stateDTO == null)
            {
                throw new NullStateDtoException();
            }

            try
            {
                var state = _mapper.Map<State>(stateDTO);
                var addedState = await _stateRepository.AddState(state);
                return _mapper.Map<StateDTO>(addedState);
            }
            catch (Exception ex)
            {
                throw new AddStateException(ex);
            }
        }

        public async Task<bool> DeleteState(int stateId)
        {
            try
            {
                var deleted = await _stateRepository.DeleteState(stateId);
                if (!deleted)
                {
                    throw new StateNotFoundException(stateId);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new DeleteStateException(stateId, ex);
            }
        }

        public async Task<List<StateDTO>> GetAllStates()
        {
            try
            {
                var states = await _stateRepository.GetAllStates();
                return _mapper.Map<List<StateDTO>>(states);
            }
            catch (Exception ex)
            {
                throw new GetAllStatesException(ex);
            }
        }

        public async Task<List<CityDTO>> GetCitiesByStateId(int stateId)
        {
            try
            {
                var cities = await _stateRepository.GetCitiesByStateId(stateId);
                return _mapper.Map<List<CityDTO>>(cities);
            }
            catch (Exception ex)
            {
                throw new GetCitiesByStateIdException(stateId, ex);
            }
        }

        public async Task<StateDTO> GetStateById(int stateId)
        {
            try
            {
                var state = await _stateRepository.GetStateById(stateId);
                if (state == null)
                {
                    throw new StateNotFoundException(stateId);
                }
                return _mapper.Map<StateDTO>(state);
            }
            catch (Exception ex)
            {
                throw new GetStateByIdException(stateId, ex);
            }
        }

        public async Task<StateDTO> UpdateState(int stateId, StateDTO stateDTO)
        {
            try
            {
                var existingState = await _stateRepository.GetStateById(stateId);
                if (existingState == null)
                {
                    throw new StateNotFoundException(stateId);
                }

                _mapper.Map(stateDTO, existingState);
                existingState.StateId = stateId; // Ensure ID is not changed

                var updatedState = await _stateRepository.UpdateState(existingState);
                return _mapper.Map<StateDTO>(updatedState);
            }
            catch (Exception ex)
            {
                throw new UpdateStateException(stateId, ex);
            }
        }
    }
}