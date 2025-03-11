using regmock.ViewModels;

namespace regmock.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            BindingContext = new RegisterViewModel();
        }
    }
}
