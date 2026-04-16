using regmock.ViewModels;

namespace regmock.Views;

public partial class RequestHelpPage : ContentPage
{
    RequestHelpPageViewModel vm;
    public RequestHelpPage()
    {
        InitializeComponent();

        vm = new RequestHelpPageViewModel();
        BindingContext = vm;

        vm.InitializeTickets();
        Service.LoggedInEvent += () =>
        {
            vm.InitializeTickets();
        };
        Service.LoggedOutEvent += () =>
        {
            vm.DeinitializeTickets();
        };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        //vm.InitializeTicketsAsync();
    }
}
