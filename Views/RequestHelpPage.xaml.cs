using regmock.ViewModels;

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

    protected override async void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        await viewModel.InitializeTicketsAsync();
    }
}
