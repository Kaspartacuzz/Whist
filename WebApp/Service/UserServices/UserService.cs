using System.Net.Http.Json;
using Core;

namespace WebApp.Service;

public class UserService : IUserService
{
    private HttpClient client;
    
    public UserService(HttpClient client)
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
    
    public async Task Update(User user)
    {
        await client.PutAsJsonAsync($"api/user/{user.Id}", user);
    }
}