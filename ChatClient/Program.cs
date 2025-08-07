using Avalonia;
using System;
using ChatClient.ViewModels;
using Avalonia.ReactiveUI;

namespace ChatClient
{
    class Program
    {
        // Avalonia entry point
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Console.WriteLine($"[UnhandledException] {e.ExceptionObject}");
            };
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UI Exception: " + ex);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .UseReactiveUI()
                         .LogToTrace();
    }
}
