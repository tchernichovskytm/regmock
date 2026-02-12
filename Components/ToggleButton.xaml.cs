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
        toggleButton.GFX.Invalidate();
    }

    public float RectRadius
    {
        get => (float)GetValue(RectRadiusProperty);
        set => SetValue(RectRadiusProperty, value);
    }

    public static readonly BindableProperty RectRadiusProperty =
          BindableProperty.Create(
              propertyName: nameof(RectRadius),
              returnType: typeof(float),
              declaringType: typeof(ToggleButton),
              defaultValue: 0.0f,
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: OnRectRadiusChanged
          );

    private static void OnRectRadiusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var toggleButton = (ToggleButton)bindable;

        toggleButton.vm.RectRadius = (float)newValue;
        toggleButton.GFX.Invalidate();
    }

    public Paint OnBackground
    {
        get => (Paint)GetValue(OnBackgroundProperty);
        set => SetValue(OnBackgroundProperty, value);
    }

    public static readonly BindableProperty OnBackgroundProperty =
          BindableProperty.Create(
              propertyName: nameof(OnBackground),
              returnType: typeof(Paint),
              declaringType: typeof(ToggleButton),
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: OnOnBackgroundChanged
          );

    private static void OnOnBackgroundChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var toggleButton = (ToggleButton)bindable;

        toggleButton.vm.OnBackground = (Paint)newValue;
        toggleButton.GFX.Invalidate();
    }

    public Paint OffBackground
    {
        get => (Paint)GetValue(OffBackgroundProperty);
        set => SetValue(OffBackgroundProperty, value);
    }

    public static readonly BindableProperty OffBackgroundProperty =
          BindableProperty.Create(
              propertyName: nameof(OffBackground),
              returnType: typeof(Paint),
              declaringType: typeof(ToggleButton),
              defaultBindingMode: BindingMode.TwoWay,
              propertyChanged: OnOffBackgroundChanged
          );

    private static void OnOffBackgroundChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var toggleButton = (ToggleButton)bindable;

        toggleButton.vm.OffBackground = (Paint)newValue;
        toggleButton.GFX.Invalidate();
    }
}