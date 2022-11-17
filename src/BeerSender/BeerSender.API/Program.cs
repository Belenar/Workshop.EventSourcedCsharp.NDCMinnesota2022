using Beersender.API.JsonConverters;
using BeerSender.API.SqlEventStream;
using BeerSender.Domain;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<EventContext>(opt =>
            {
                opt.UseSqlServer("Data Source=localhost;Initial Catalog=EventStore;User ID=sa;Password=Password1!;TrustServerCertificate=True;");
            });

            builder.Services.AddScoped<SqlEventStream.SqlEventStream>();
            
            builder.Services.AddScoped<CommandRouter>(provider =>
            {
                var sqlStream = provider.GetService<SqlEventStream.SqlEventStream>();
                return new CommandRouter(sqlStream.GetEvents, sqlStream.PublishEvent);
            });
            
            builder.Services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new CommandConverter()));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}