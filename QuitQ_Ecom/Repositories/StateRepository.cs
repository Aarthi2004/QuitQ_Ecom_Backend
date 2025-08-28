using Microsoft.EntityFrameworkCore;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuitQ_Ecom.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly QuitQEcomContext _context;

        public StateRepository(QuitQEcomContext context)
        {
            _context = context;
        }

        public async Task<State> AddState(State state)
        {
            _context.States.Add(state);
            await _context.SaveChangesAsync();
            return state;
        }

        public async Task<bool> DeleteState(int stateId)
        {
            var state = await _context.States.FindAsync(stateId);
            if (state == null)
                return false;

            _context.States.Remove(state);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<State>> GetAllStates()
        {
            return await _context.States.ToListAsync();
        }

        public async Task<List<City>> GetCitiesByStateId(int stateId)
        {
            return await _context.Cities.Where(c => c.StateId == stateId).ToListAsync();
        }

        public async Task<State> GetStateById(int stateId)
        {
            return await _context.States.FindAsync(stateId);
        }

        public async Task<State> UpdateState(State state)
        {
            _context.Entry(state).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return state;
        }
    }
}