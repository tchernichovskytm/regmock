namespace regmock.Components;

public partial class ToggleButton : ContentView
{
    public ToggleButtonViewModel vm;
    public ToggleButton()
    {
        InitializeComponent();

        Command ViewModelToggleCmd = new Command(() =>
        {
            IsToggled = !IsToggled;
        });
        vm = new ToggleButtonViewModel(ViewModelToggleCmd);
        BindingContext = vm;
    }

    public bool IsToggled
    {
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
        toggleButton.InvalidateMeasure();
    }
}