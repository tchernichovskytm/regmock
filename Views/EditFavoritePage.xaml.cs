using regmock.Models;
using regmock.ViewModels;

namespace regmock.Views;

public partial class EditFavoritePage : ContentPage
{
    public EditFavoritePage(Favorite favorite, Command favoriteCmd)
    {
        InitializeComponent();

        BindingContext = new EditFavoritePageViewModel(favorite, favoriteCmd);
    }
}