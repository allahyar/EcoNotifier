using Services;
using Tasks;
using Utilities;

class MainProgram
{
    static async Task Main(string[] args)
    {
        AppManager.HideConsoleWindow();
        RegistryManager.RegisterApplicationInStartup();

        await EventManager.FetchEvents();
        TimerManager.InitializeDailyTimer();

        await EventManager.StartEventProcessingTask();
    }
}