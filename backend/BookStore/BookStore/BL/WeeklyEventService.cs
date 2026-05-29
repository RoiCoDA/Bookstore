using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace BookStore.BL
{
    public class WeeklyEventService : IHostedService, IDisposable
    {
        private readonly ILogger<WeeklyEventService> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;
        private Timer _timer;

        public WeeklyEventService(ILogger<WeeklyEventService> logger, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Weekly Event Service started.");

            DateTime now = DateTime.Now;

            DayOfWeek targetDay = DayOfWeek.Thursday;
            TimeSpan targetTimeOfDay = new TimeSpan(19, 0, 0);

            int daysUntilTarget = ((int) targetDay - (int) now.DayOfWeek + 7) % 7;

            DateTime nextEvent = now.Date.AddDays(daysUntilTarget).Add(targetTimeOfDay);

            if ( daysUntilTarget == 0 && now.TimeOfDay > targetTimeOfDay )
            {
                nextEvent = nextEvent.AddDays(7);
            }

            TimeSpan timeToGo = nextEvent - now;

            _timer = new Timer(ExecuteEvent, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            ScheduleNextRun();

            return Task.CompletedTask;
        }

        private async void ExecuteEvent(object state)
        {
            try
            {
                _logger.LogInformation($"Event started at: {DateTime.Now}");

                DBservices dbs = new DBservices();
                var discountedBooks = await dbs.Apply5BookDiscountForSale();

                ////Console.WriteLine("Books after discount:");
                ////Console.WriteLine(JsonSerializer.Serialize(discountedBooks, new JsonSerializerOptions { WriteIndented = true }));

                // Notify all WebSocket clients about the sale start

                var saleStartMessage = new
                {
                    @event = "SaleStart",
                    data = new { message = "Sale has started!", books = discountedBooks }
                };
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", saleStartMessage);


                await Task.Delay(TimeSpan.FromMinutes(15));

                bool success = await dbs.Revert5BookDiscoutForSale();
                _logger.LogInformation($"Reversion success: {success}");

                var saleEndMessage = new
                {
                    @event = "SaleEndMessage",
                    data = new { message = "Sale has ended!" }
                };

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", saleEndMessage);

                ScheduleNextRun();

                //Console.WriteLine($"Event ended at: {DateTime.Now}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in weekly sale event");
            }
        }

        private void ScheduleNextRun()
        {
            DateTime now = DateTime.Now;

            DayOfWeek targetDay = DayOfWeek.Thursday;
            TimeSpan targetTimeOfDay = new TimeSpan(19, 0, 0);

            int daysUntilTarget = ((int)targetDay - (int)now.DayOfWeek + 7) % 7;

            DateTime nextEvent = now.Date.AddDays(daysUntilTarget).Add(targetTimeOfDay);

            if (daysUntilTarget == 0 && now.TimeOfDay > targetTimeOfDay)
            {
                nextEvent = nextEvent.AddDays(7);
            }

            TimeSpan timeToGo = nextEvent - now;

            _timer?.Change(timeToGo, Timeout.InfiniteTimeSpan);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Weekly Event Service stopped.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
