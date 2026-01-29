using regmock.ViewModels;

namespace regmock.Views;

public partial class PickFavoritesPage : ContentPage
{
    PickFavoritesPageViewModel vm;
    public PickFavoritesPage()
    {
        InitializeComponent();

        vm = new PickFavoritesPageViewModel();
        BindingContext = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        vm.FetchFavorites();
    }
}