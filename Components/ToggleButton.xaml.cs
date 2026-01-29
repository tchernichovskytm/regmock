namespace regmock.Components;

public partial class ToggleButton : ContentView
{
	public ToggleButton()
	{
		InitializeComponent();

		BindingContext = new ToggleButtonViewModel();
	}
}