using Timer = System.Threading.Timer;

namespace Tasks
{
    class TimerManager
    {
        private static Timer timer;

        public static void InitializeDailyTimer()
        {
            
            DateTime nextRunTime = DateTime.Now.Date.AddDays(1).AddMinutes(15);
            TimeSpan timeToGo = nextRunTime - DateTime.Now;

            timer = new Timer(async _ => await ExecuteDailyTask(), null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private static async Task ExecuteDailyTask()
        {
            await EventManager.FetchEvents();

            InitializeDailyTimer();
        }
    }
}