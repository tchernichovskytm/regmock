using regmock.Models;
using regmock.Views;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

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

        private bool isEditing;

        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }
        #endregion

        #region Commands
        public ICommand TicketToggleCmd { get; set; }
        public ICommand AddTicketCmd { get; set; }
        public ICommand EditTicketsCmd { get; set; }
        public ICommand DeleteTicketCmd { get; set; }
        #endregion

        #region Constructor
        public async Task InitializeTicketsAsync()
        {
            // get all the items from the fb into the service
            await Service.GetAllTicketsFromFB();
            // get the new ticket list from the service
            Tickets = new ObservableCollection<Ticket>(Service.GetSelfTickets());
            OnPropertyChanged(nameof(Tickets));

            BringTopicsToFirst(Tickets);

            foreach (Ticket ticket in Tickets)
            {
                ticket.IsActiveToggleCmd = (Command)TicketToggleCmd;
                ticket.DeleteCmd = (Command)DeleteTicketCmd;
            }
        }

        private Stopwatch ticketStopwatch = new Stopwatch();

        public RequestHelpPageViewModel()
        {
            TicketToggleCmd = new Command((object obj) =>
            {
                if (obj is Ticket)
                {
                    Ticket ticket = (Ticket)obj;
                    TicketToggled(ticket);
                }
            });

            DeleteTicketCmd = new Command((object obj) =>
            {
                if (obj is Ticket)
                {
                    Ticket ticket = (Ticket)obj;
                    DeleteTicket(ticket);
                }
            });

            AddTicketCmd = new Command(HandleTicket);

            EditTicketsCmd = new Command(() =>
            {
                IsEditing = !IsEditing;
            });

            ticketStopwatch.Start();
            Thread clock = new Thread(TimerDecrease);
            clock.Start();
        }
        #endregion

        #region Functions
        private async void DeleteTicket(Ticket ticket)
        {
            var success = await Service.DeleteTicket(ticket);
            if (!success)
            {
                // TODO: handle error
            }
            Tickets.Remove(ticket);
        }

        private async void HandleTicket()
        {
            ICommand ticketCmd = new Command(async (object newTicketObj) =>
            {
                Monitor.Enter(this);
                if (newTicketObj is Ticket)
                {
                    Ticket newTicket = (Ticket)newTicketObj;
                    newTicket.IsActiveToggleCmd = TicketToggleCmd;
                    var success = await Service.HandleTicket(newTicket);
                    if (!success)
                    {
                        // TODO: handle error
                    }
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
            var success = await Service.HandleTicket(ticket);
            if (success)
            {
                if (ticket.IsActive == true)
                {
                    ticket.ActiveTimeSpan = Service.UnixMiliseconds24Hours;
                    ticket.ServerActiveTime = Service.UnixMilisecondsToHHMMSS(ticket.ActiveTimeSpan);
                }
                else
                {
                    ticket.ActiveTimeSpan = 0;
                    ticket.ServerActiveTime = "";
                }
            }
            else
            {
                // TODO: handle error
            }
            Monitor.Exit(this);
        }

        public void BringTopicsToFirst(ObservableCollection<Ticket> tickets)
        {
            foreach (Ticket ticket in tickets)
            {
                ticket.Topics = new List<string> { ticket.Topics.LastOrDefault() };
            }
        }

        public async void TimerDecrease()
        {
            while (true)
            {
                await Task.Delay(1000);
                Int64 ellapsed = ticketStopwatch.ElapsedMilliseconds;
                ticketStopwatch.Restart();
                if (Tickets == null) continue;

                Monitor.Enter(this);
                foreach (Ticket ticket in Tickets)
                {
                    if (ticket.IsActive == false)
                    {
                        continue;
                    }
                    ticket.ActiveTimeSpan = ticket.ActiveTimeSpan -= ellapsed; // 1000ms == 1s
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
