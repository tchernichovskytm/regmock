namespace regmock.Views;
using regmock.ViewModels;

public partial class Testing : ContentPage
{
	public Testing()
	{
		InitializeComponent();

		BindingContext = new TestingVM();
	}
}