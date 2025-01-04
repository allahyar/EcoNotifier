using Microsoft.Win32;

namespace Services
{
    class RegistryManager
    {
        public static void RegisterApplicationInStartup()
        {
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key == null)
                    {
                        Console.WriteLine("Registry key not found.");
                        return;
                    }
                    key.SetValue("EcoNotifier", exePath);
                    Console.WriteLine("Application added to startup.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}