using ExpandeBO_Backend.Models;
using MongoDB.Driver;

namespace ExpandeBO_Backend.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly IMongoCollection<Plan> _planes;

    public PlanRepository(IMongoContext context)
    {
        _planes = context.Database.GetCollection<Plan>("planes");
    }

    public async Task<Plan?> GetByIdAsync(string id)
    {
        return await _planes.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Plan> CreateAsync(Plan plan)
    {
        await _planes.InsertOneAsync(plan);
        return plan;
    }

    public async Task<Plan> UpdateAsync(Plan plan)
    {
        await _planes.ReplaceOneAsync(p => p.Id == plan.Id, plan);
        return plan;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _planes.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Plan>> GetAllAsync()
    {
        return await _planes.Find(_ => true).ToListAsync();
    }

    public async Task<List<Plan>> GetActivosAsync()
    {
        return await _planes.Find(p => p.Activo).ToListAsync();
    }
}


