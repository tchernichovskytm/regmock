using regmock.Models;
using regmock.Views;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace regmock.ViewModels
{
    public class RequestHelpPageViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<Ticket> tickets;

        public ObservableCollection<Ticket> Tickets
        {
            get { return tickets; }
            set
            {
                tickets = value;
                OnPropertyChanged(nameof(Tickets));
            }
        }
        #endregion

        #region Commands
        public ICommand TicketToggleCmd { get; set; }
        public ICommand AddTicketCmd { get; set; }
        #endregion

        #region Constructor
        public async Task InitializeTicketsAsync()
        {
            // get all the items from the fb into the service
            await Service.GetAllTicketsFromFB();
            // get the new ticket list from the service
            Tickets = new ObservableCollection<Ticket>(Service.GetSelfTickets());

            BringTopicsToFirst();

            foreach (Ticket ticket in Tickets)
            {
                ticket.IsActiveToggleCmd = (Command)TicketToggleCmd;
            }

            Thread clock = new Thread(TimerDecrease);
            clock.Start();

        }

        public RequestHelpPageViewModel()
        {

            TicketToggleCmd = new Command((object ticket) =>
            {
                if (ticket is Ticket)
                {
                    TicketToggled((Ticket)ticket);
                }
            });

            AddTicketCmd = new Command(NewTicketClick);
        }
        #endregion

        #region Functions
        private async void NewTicketClick()
        {
            ICommand ticketCmd = new Command((newTicketObj) =>
            {
                Monitor.Enter(this);
                if (newTicketObj is Ticket)
                {
                    Ticket newTicket = (Ticket)newTicketObj;
                    Tickets.Add(newTicket);
                    newTicket.IsActiveToggleCmd = (Command)TicketToggleCmd;
                }
                Monitor.Exit(this);
            });
            await Shell.Current.Navigation.PushModalAsync(new NewTicketPage((Command)ticketCmd), true);

            // DONT: do not send ticket back to service
        }
        public async void TicketToggled(Ticket ticket)
        {
            Monitor.Enter(this);
            var response = await Service.HandleTicket(ticket);
            if (response)
            {
                if (ticket.IsActive == true)
                {
                    ticket.ActiveTimeSpan = TimeSpan.FromHours(24);
                    ticket.ServerActiveTime = Service.TimeSpanToString(ticket.ActiveTimeSpan);
                }
                else if (ticket.IsActive == false)
                {
                    ticket.ActiveTimeSpan = TimeSpan.Zero;
                    ticket.ServerActiveTime = "";
                }
            }
            Monitor.Exit(this);
        }

        public void BringTopicsToFirst()
        {
            foreach (Ticket ticket in Tickets)
            {
                ticket.Topics = new List<string> { ticket.Topics.LastOrDefault() };
            }
        }

        public async void TimerDecrease()
        {
            while (true)
            {
                await Task.Delay(1000);
                Monitor.Enter(this);
                foreach (Ticket ticket in Tickets)
                {
                    if (ticket.IsActive == false)
                    {
                        continue;
                    }
                    ticket.ActiveTimeSpan = ticket.ActiveTimeSpan.Subtract(new TimeSpan(0, 0, 1));
                    if (ticket.ActiveTimeSpan.CompareTo(TimeSpan.Zero) <= 0)
                    {
                        ticket.IsActive = false;
                        ticket.ServerActiveTime = "";
                    }
                    else
                    {
                        ticket.ServerActiveTime = ticket.ActiveTimeSpan.ToString(@"hh\:mm\:ss");
                    }
                }
                Monitor.Exit(this);
            }
        }
        #endregion
    }
}
