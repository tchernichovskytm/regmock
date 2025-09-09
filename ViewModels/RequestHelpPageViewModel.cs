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

            TicketToggleCmd = new Command(async (object obj) =>
            {
                if (obj is Ticket)
                {
                    Ticket ticket = (Ticket)obj;
                    TicketToggled(ticket);
                }
            });

            AddTicketCmd = new Command(NewTicketClick);
        }
        #endregion

        #region Functions
        private async void NewTicketClick()
        {
            ICommand ticketCmd = new Command((object newTicketObj) =>
            {
                Monitor.Enter(this);
                if (newTicketObj is Ticket)
                {
                    Ticket newTicket = (Ticket)newTicketObj;
                    newTicket.IsActiveToggleCmd = TicketToggleCmd;
                    Tickets.Add(newTicket);
                }
                Monitor.Exit(this);
            });
            await Shell.Current.Navigation.PushModalAsync(new NewTicketPage((Command)ticketCmd), true);

            // DONT: do not send ticket back to service
        }
        public async void TicketToggled(Ticket ticket)
        {
            Monitor.Enter(this);
            var (success, _) = await Service.HandleTicket(ticket);
            if (success)
            {
                if (ticket.IsActive == true)
                {
                    ticket.ActiveTimeSpan = Service.Unix24HoursMiliseconds;
                    ticket.ServerActiveTime = Service.UnixMilisecondsToHHMMSS(ticket.ActiveTimeSpan);
                }
                else
                {
                    ticket.ActiveTimeSpan = 0;
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
                    ticket.ActiveTimeSpan = ticket.ActiveTimeSpan -= 1000; // 1000ms == 1s
                    if (ticket.ActiveTimeSpan <= 0)
                    {
                        ticket.IsActive = false;
                        ticket.ServerActiveTime = "";
                    }
                    else
                    {
                        ticket.ServerActiveTime = Service.UnixMilisecondsToHHMMSS(ticket.ActiveTimeSpan);
                    }
                }
                Monitor.Exit(this);
            }
        }
        #endregion
    }
}
