using Core;

namespace ServerAPI.Repositories.Points;

public interface IPointRepository
{
    Task<List<PointEntry>> GetAll();
    Task Add(PointEntry point);
    Task Delete(int id);
    Task<int> GetNextId();
    Task DeleteAll(); // hvis vi skal nulstille point
}