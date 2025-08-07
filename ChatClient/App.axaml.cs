using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ChatClient.Views; 
using ChatClient.ViewModels;
using Avalonia.Controls;
using System;

namespace ChatClient
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var loginWindow = new LoginWindowView();

                loginWindow.LoginCompleted += username =>
                {
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        Window window;

                        if (username.ToLower() == "admin")
                        {
                            
                            window = new Window
                            {
                                Title = "Admin Panel",
                                Content = new AdminView(),
                                Width = 800,
                                Height = 600
                            };
                        }
                        else
                        {
                            var mainWindow = new MainWindow
                            {
                                DataContext = new ChatViewModel(username)
                            };
                            window = mainWindow;
                        }

                        desktop.MainWindow = window;
                        window.Show();
                    }

                    loginWindow.Close();
                };

                loginWindow.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }


    }
}
