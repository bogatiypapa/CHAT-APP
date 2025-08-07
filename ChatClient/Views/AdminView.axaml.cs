using Avalonia.Controls;
using ChatClient.ViewModels;

namespace ChatClient.Views
{
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
            DataContext = new AdminViewModel();
        }
    }
}
