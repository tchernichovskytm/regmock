using regmock.ViewModels;

namespace regmock.Views;

public partial class GiveHelpPage : ContentPage
{
    private GiveHelpPageViewModel viewModel;
    public GiveHelpPage()
    {
        InitializeComponent();

        viewModel = new GiveHelpPageViewModel();
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        await viewModel.InitializeTicketsAsync();
    }
}