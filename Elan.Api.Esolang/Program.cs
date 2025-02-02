using Elan.Api.Esolang.Repositories.Interfaces;
using Elan.Api.Esolang.Services;

namespace Elan.Api.Esolang
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(
                options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        });
                });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<Repositories.Interfaces.ISparqlRepository, Repositories.SparqlRepository>();
            builder.Services.AddScoped<IWikidataService, WikidataService>();
            builder.Services.AddScoped<EsolangService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var esolangService = scope.ServiceProvider.GetRequiredService<EsolangService>();
                await esolangService.InitializeCacheAsync();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
