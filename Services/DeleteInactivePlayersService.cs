using ChessServer.Models;

namespace ChessServer.Services
{
    public class DeleteInactivePlayersService : IHostedService, IDisposable
    {
        private readonly ILogger<DeleteInactivePlayersService> _logger;
        private Timer? _timer = null;
        private readonly IServiceScopeFactory _scopeFactory;

        public DeleteInactivePlayersService(ILogger<DeleteInactivePlayersService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ChessDBContext>();

                if (dbContext != null)
                {
                    try
                    {
                        foreach (var player in dbContext.Players)
                        {
                            player.InactiveCounter += 1;
                            if (player.InactiveCounter == 5)
                            {
                                dbContext.Players.Remove(player);
                                dbContext.Invitations.RemoveRange(dbContext.Invitations.Where(a => a.PlayerId == player.Id));
                            }
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    catch
                    {
                        ;
                    }
                    try
                    {
                        foreach (var game in dbContext.Games)
                        {
                            game.WhiteInactivityCounter += 1;
                            game.BlackInactivityCounter += 1;
                            if (game.WhiteInactivityCounter == 5 || game.BlackInactivityCounter == 5)
                            {
                                game.HasPlayerQuit = true;
                            }
                            if (game.WhiteInactivityCounter >= 5 && game.BlackInactivityCounter >= 5)
                            {
                                dbContext.Games.Remove(game);
                            }
                            await dbContext.SaveChangesAsync();
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
