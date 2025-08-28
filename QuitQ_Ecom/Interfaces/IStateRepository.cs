using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Interfaces
{
    public interface IStateRepository
    {
        Task<List<State>> GetAllStates();
        Task<List<City>> GetCitiesByStateId(int stateId);
        Task<State> GetStateById(int stateId);
        Task<State> AddState(State state);
        Task<State> UpdateState(State state);
        Task<bool> DeleteState(int stateId);
    }
}