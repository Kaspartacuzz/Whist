using System.Net.Http.Json;
using Core;

namespace WebApp.Service;

public class UserServiceMock : IUserService
{
    private HttpClient client;
    
    public UserServiceMock(HttpClient client)
    {
        this.client = client;
    }
    
    public async Task<User[]> GetAll()
    {
        return await client.GetFromJsonAsync<User[]>("api/user");
    }
    
    public async Task<User?> GetById(int id)
    {
        return await client.GetFromJsonAsync<User?>($"api/user/{id}");
    }

    public async Task AddUser(User user)
    { 
        await client.PostAsJsonAsync("api/user", user);       
    }
    
    public async Task Delete(int id)
    {
        await client.DeleteAsync($"api/user/{id}");       
    }
}