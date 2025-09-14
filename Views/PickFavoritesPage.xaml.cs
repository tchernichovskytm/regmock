using regmock.ViewModels;

namespace regmock.Views;

public partial class PickFavoritesPage : ContentPage
{
    private PickFavoritesPageViewModel viewModel;
    public PickFavoritesPage()
    {
        InitializeComponent();

        viewModel = new PickFavoritesPageViewModel();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InitializeFavoritesAsync();
    }
}