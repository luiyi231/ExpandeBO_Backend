using ExpandeBO_Backend.Models;

namespace ExpandeBO_Backend.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(string id);
    Task<Plan> CreateAsync(Plan plan);
    Task<Plan> UpdateAsync(Plan plan);
    Task<bool> DeleteAsync(string id);
    Task<List<Plan>> GetAllAsync();
    Task<List<Plan>> GetActivosAsync();
}


