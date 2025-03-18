using regmock.ViewModels;

namespace regmock.Views;

public partial class NewPreferencePage : ContentPage
{
	public NewPreferencePage(Command preferenceCmd)
	{
		InitializeComponent();

		BindingContext = new NewPreferencePageViewModel(preferenceCmd);
	}
}