using regmock.ViewModels;
using regmock.Models;

namespace regmock.Views;
public partial class RequestHelpPage : ContentPage
{
    private RequestHelpPageViewModel viewModel;
    public RequestHelpPage()
    {
        InitializeComponent();

        viewModel = new RequestHelpPageViewModel();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InitializeTicketsAsync();
    }
}
