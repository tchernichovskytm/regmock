namespace regmock.Components;

public partial class ToggleButton : ContentView
{
    // this has to be static because OnToggled is static
    // i hate C#
    public ToggleButtonViewModel vm;
    public ToggleButton()
    {
        InitializeComponent();

        vm = new ToggleButtonViewModel();
        BindingContext = vm;
    }

    public bool IsToggled
    {
        //get { return vm.IsToggled; }
        //set { vm.IsToggled = value; }
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public static readonly BindableProperty IsToggledProperty =
          BindableProperty.Create(
              propertyName: nameof(IsToggled),
              returnType: typeof(bool),
              declaringType: typeof(ToggleButton),
              defaultValue: false,
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: OnToggled
          );

    private static void OnToggled(BindableObject bindable, object oldValue, object newValue)
    {
        var toggleButton = (ToggleButton)bindable;

        toggleButton.vm.IsToggled = (bool)newValue;
    }
}