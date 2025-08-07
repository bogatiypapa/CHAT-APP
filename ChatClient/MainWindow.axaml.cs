using System;
using Avalonia.Controls;
using ChatClient.ViewModels;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        public MainWindow(string username)
        {
            InitializeComponent();
            DataContext = new ChatViewModel(username);

        }

    }
}
