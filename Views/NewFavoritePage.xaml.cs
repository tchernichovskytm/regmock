using regmock.Models;
using regmock.ViewModels;

namespace regmock.Views;

public partial class NewFavoritePage : ContentPage
{
    public NewFavoritePage(List<Subject> ExistingSubjects, Command favoriteCmd)
    {
        InitializeComponent();

        BindingContext = new NewFavoritePageViewModel(ExistingSubjects, favoriteCmd);
    }
}