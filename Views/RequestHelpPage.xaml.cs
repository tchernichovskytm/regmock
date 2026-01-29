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
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs e)
    {
        base.OnNavigatedTo(e);
        vm.FetchTickets();
    }
}
