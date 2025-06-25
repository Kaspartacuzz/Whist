using Core;

namespace ServerAPI.Repositories;

public interface IUserRepository
{
    User[] GetAll();
    User? GetById(int id);
    void AddUser(User user);
    void Delete(int id);
}