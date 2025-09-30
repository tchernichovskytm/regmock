namespace regmock
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell()
            {
                FlowDirection = FlowDirection.LeftToRight,
            };

            // TODO: dark theme not supported yet
            Application.Current.UserAppTheme = AppTheme.Light;
        }
    }
}
