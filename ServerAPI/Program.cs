using ServerAPI.Repositories;
using ServerAPI.Repositories.Fines;
using ServerAPI.Repositories.Highlights;
using ServerAPI.Repositories.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserRepository, UserRepositoryMongoDB>();
builder.Services.AddSingleton<IFineRepository, FineRepositoryMongoDB>();
builder.Services.AddSingleton<IHighlightRepository, HighlightRepositoryMongoDB>();
builder.Services.AddSingleton<IRuleRepository, RuleRepositoryMongoDB>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5102")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();