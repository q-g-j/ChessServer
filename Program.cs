using Microsoft.EntityFrameworkCore;

using ChessServer.Models;
using ChessServer.Services;

namespace ChessServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Add services to the container.
            builder.Services.AddDbContext<ChessDBContext>(opt => opt.UseInMemoryDatabase("Chess"));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddHostedService<DeleteInactivePlayersService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}