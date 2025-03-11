using regmock.ViewModels;

namespace regmock.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        BindingContext = new LoginViewModel();
    }
}