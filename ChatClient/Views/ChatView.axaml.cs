using Avalonia.Controls;
using System;

namespace ChatClient.Views
{
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
            this.DataContextChanged += (_, __) =>
            {
                Console.WriteLine($"ChatView DataContext: {DataContext}");
            };
        }
    }
}
