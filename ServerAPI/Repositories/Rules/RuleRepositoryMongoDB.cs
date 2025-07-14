using Core;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Repositories.Rules;

public class RuleRepositoryMongoDB : IRuleRepository
{
    private readonly IMongoCollection<Rule> _rules;

    public RuleRepositoryMongoDB(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
        _rules = db.GetCollection<Rule>("Rules");
    }

    public async Task<List<Rule>> GetAll() =>
        await _rules.Find(_ => true).ToListAsync();

    public async Task<Rule?> GetById(int id) =>
        await _rules.Find(r => r.Id == id).FirstOrDefaultAsync();

    public async Task<Rule> Add(Rule rule)
    {
        // Find næste ledige Id (manuelt, da vi ikke bruger ObjectId)
        var highest = await _rules.Find(_ => true).SortByDescending(r => r.Id).FirstOrDefaultAsync();
        rule.Id = highest?.Id + 1 ?? 1;

        await _rules.InsertOneAsync(rule);
        return rule;
    }

    public async Task Update(Rule rule) =>
        await _rules.ReplaceOneAsync(r => r.Id == rule.Id, rule);

    public async Task Delete(int id) =>
        await _rules.DeleteOneAsync(r => r.Id == id);
}
