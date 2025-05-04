using Core;

namespace ServerAPI.Repositories;

public interface IUserRepository
{
    User[] GetAll();
    User? GetById(int id);
    void Add(User user);
    void Delete(int id);
}