using regmock.ViewModels;

namespace regmock
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            BindingContext = new ShellViewModel();
        }
    }
}
