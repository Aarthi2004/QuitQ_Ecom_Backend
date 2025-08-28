using QuitQ_Ecom.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IStateService
    {
        Task<List<StateDTO>> GetAllStates();
        Task<List<CityDTO>> GetCitiesByStateId(int stateId);
        Task<StateDTO> GetStateById(int stateId);
        Task<StateDTO> AddState(StateDTO stateDTO);
        Task<StateDTO> UpdateState(int stateId, StateDTO stateDTO);
        Task<bool> DeleteState(int stateId);
    }
}