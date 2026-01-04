using System.Windows.Input;
using regmock.ViewModels;

namespace regmock
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
