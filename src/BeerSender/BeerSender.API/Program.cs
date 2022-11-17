using Beersender.API.JsonConverters;
using BeerSender.API.Sql_event_stream;
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
            builder.Services.AddScoped<Sql_event_stream.Sql_event_stream>();
            builder.Services.AddScoped<Command_router>(provider =>
            {
                var sql_stream = provider.GetService<Sql_event_stream.Sql_event_stream>();
                return new Command_router(sql_stream.Get_events, sql_stream.Publish_event);
            });
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