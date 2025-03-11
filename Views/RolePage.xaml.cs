using regmock.ViewModels;

namespace regmock.Views;

public partial class RolePage : ContentPage
{
	public RolePage()
	{
		InitializeComponent();

		BindingContext = new RolePageViewModel();
	}
}