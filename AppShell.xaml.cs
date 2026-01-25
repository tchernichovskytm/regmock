using regmock.ViewModels;
using System.ComponentModel;

namespace regmock
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            BindingContext = new AppShellViewModel();
        }
    }
}
