using regmock.ViewModels;
using regmock.Models;

namespace regmock.Views;
public partial class RequestHelpPage : ContentPage
{
    private RequestHelpPageViewModel viewModel;
    private bool firstLoad = false;
    public RequestHelpPage()
    {
        InitializeComponent();

        viewModel = new RequestHelpPageViewModel();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        if (firstLoad == false)
        {
            base.OnAppearing();
            await viewModel.InitializeTicketsAsync();
            firstLoad = true;
        }
    }
}
