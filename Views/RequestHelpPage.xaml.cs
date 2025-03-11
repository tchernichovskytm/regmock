using regmock.ViewModels;
using regmock.Models;

namespace regmock.Views;
public partial class RequestHelpPage : ContentPage
{
    public RequestHelpPage()
    {
        InitializeComponent();

        BindingContext = new RequestHelpPageViewModel();
    }
}
