using regmock.Models;
using CommunityToolkit.Maui.Views;
using System.Threading.Tasks;

namespace regmock.Components;

public partial class ContactPopup : Popup
{
    public ContactPopup(Ticket ticket)
    {
        InitializeComponent();

        BindingContext = new ContactPopupViewModel(this, ticket);
    }
}