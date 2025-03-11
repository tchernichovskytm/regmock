using regmock.ViewModels;

namespace regmock.Views;

public partial class GiveHelpPage : ContentPage
{
	public GiveHelpPage()
	{
		InitializeComponent();

		BindingContext = new GiveHelpPageViewModel();
	}
}