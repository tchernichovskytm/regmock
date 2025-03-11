namespace regmock.Views;
using regmock.ViewModels;
using System.Runtime.CompilerServices;

public partial class NewTicketPage : ContentPage
{
	public NewTicketPage(Command ticketCmd)
	{
		InitializeComponent();

		BindingContext = new NewTicketPageViewModel(ticketCmd);
	}
}