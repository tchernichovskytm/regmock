using regmock.ViewModels;

namespace regmock.Views;

public partial class RequestHelpPage : ContentPage
{
    public RequestHelpPage()
    {
        InitializeComponent();

        BindingContext = new RequestHelpPageViewModel();
    }
}
