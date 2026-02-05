using System.Windows.Input;
using regmock.ViewModels;
using regmock.Views;

namespace regmock
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new Testing();
        }
    }
}
