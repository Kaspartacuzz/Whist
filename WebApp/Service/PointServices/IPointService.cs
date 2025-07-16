using Core;

namespace WebApp.Service.PointServices;

public interface IPointService
{
    Task<List<PointEntry>> GetAll();
    Task Add(PointEntry point);
    Task Delete(int id);
    Task DeleteAll();
}