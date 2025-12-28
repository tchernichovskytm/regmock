using regmock.ViewModels;

namespace regmock.Views;

public partial class PickFavoritesPage : ContentPage
{
    public PickFavoritesPage()
    {
        InitializeComponent();

        BindingContext = new PickFavoritesPageViewModel();
    }
}