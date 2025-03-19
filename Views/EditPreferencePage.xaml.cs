using regmock.Models;
using regmock.ViewModels;

namespace regmock.Views;

public partial class EditPreferencePage : ContentPage
{
	public EditPreferencePage(Favorite favorite, Command favoriteCmd)
	{
		InitializeComponent();

		BindingContext = new EditPreferencePageViewModel(favorite, favoriteCmd);
    }
}