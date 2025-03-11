using System.Windows.Input;

namespace regmock.Behaviors
{
    public class SwitchBehavior : Behavior<Switch>
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create
            (
                nameof(Command),
                typeof(ICommand),
                typeof(SwitchBehavior)
            );

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttachedTo(Switch bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Toggled += OnToggled;
        }

        protected override void OnDetachingFrom(Switch bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Toggled -= OnToggled;
        }

        private void OnToggled(object sender, ToggledEventArgs e)
        {
            if (Command?.CanExecute(e.Value) == true)
            {
                Command.Execute(e.Value);
            }
        }
    }
}
