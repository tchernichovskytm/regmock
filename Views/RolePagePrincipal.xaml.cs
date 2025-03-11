namespace regmock.Views;
using regmock.ViewModels;

public partial class RolePagePrincipal : ContentPage
{
	public RolePagePrincipal()
	{
		InitializeComponent();

		BindingContext = new RolePagePrincipalViewModel();
	}
}