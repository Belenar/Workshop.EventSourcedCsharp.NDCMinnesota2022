using Beersender.API.JsonConverters;
using BeerSender.API.Projections;
using BeerSender.API.Projections.Infrastructure;
using BeerSender.API.Read_models;
using BeerSender.API.Sql_event_stream;
using BeerSender.Domain;
using BeerSender.Domain.Infrastructure;
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
                opt.UseSqlServer(
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EventStore;Integrated Security=True");
            });

            builder.Services.AddDbContext<Read_context>(opt =>
            {
                opt.UseSqlServer(
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ReadModel;Integrated Security=True");
            });
            builder.Services.AddHostedService<Projection_service<Beer_package_projection>>();
            builder.Services.AddScoped<IEventStream, Sql_event_stream.Sql_event_stream>();
            builder.Services.AddScoped<IAggregateCache, AggregateCache>();
            builder.Services.AddScoped<Command_router>();
            builder.Services.AddControllers()
                .AddJsonOptions(opt => 
                    opt.JsonSerializerOptions.Converters.Add(new CommandConverter()));
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