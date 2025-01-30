
using ELAN.Api.Repositories;
using ELAN.Api.Services;

namespace ELAN.Api
{
    public class Program
    {
        public static void Main(string[] args)
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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<Repositories.Interfaces.ISparqlRepository, Repositories.SparqlRepository>();
            builder.Services.AddSingleton<OntologyRepository>(provider =>
            {
                var ontologyPath = Path.Combine(AppContext.BaseDirectory, "Data/ontology.ttl");
                return new OntologyRepository(ontologyPath);
            });
            builder.Services.AddScoped<WikidataService>();
            builder.Services.AddScoped<EsolangService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
