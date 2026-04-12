using Microsoft.EntityFrameworkCore;
using Tce.Api.Endpoints;
using Tce.Api.Extensions;
using Tce.Infrastructure.Data.Context;
using Tce.Infrastructure.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    var retries = 10;
    while (retries > 0)
    {
        try
        {
            db.Database.Migrate();
            DbInitializer.Seed(db);

            Console.WriteLine("SEED EXECUTADO COM SUCESSO");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao aplicar seed: {ex.Message}");

            retries--;
            Thread.Sleep(5000);
        }
    }
}

//TODO: Esta comentado para expor a documentação da API, mas em produção isso deve ser habilitado
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.MapGet("/", () => "TCE API Running");

app.UseCors("AllowAll");

app.MapTransactionEndpoints();
app.MapCategoryEndpoints();

app.Run();
