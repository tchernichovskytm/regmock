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

        //private TimeSpan timer;

        //public TimeSpan Timer
        //{
        //    get { return timer; }
        //    set
        //    {
        //        timer = value;
        //        OnPropertyChanged(nameof(TimerStr));
        //    }
        //}
        //public string TimerStr
        //{
        //    get { return Timer.ToString(); }
        //}
        #endregion

        #region Commands
        public ICommand TicketToggleCmd { get; set; }
        public ICommand AddTicketCmd { get; set; }
        #endregion

        #region Constructor
        public RequestHelpPageViewModel()
        {
            // get the list of those new tickets
            Task<List<Ticket>> ticketsList = Service.GetAllTickets();
            Tickets = new ObservableCollection<Ticket>(ticketsList.Result);

            BringTopicsToFirst();
            
            TicketToggleCmd = new Command((object ticket) =>
            {
                if (ticket is Ticket)
                {
                    TicketToggled((Ticket)ticket);
                }
            });
            foreach (Ticket ticket in Tickets)
            {
                ticket.IsActiveToggleCmd = (Command)TicketToggleCmd;
            }

            Thread clock = new Thread(TimerDecrease);
            clock.Start();

            AddTicketCmd = new Command(NewTicketClick);
        }
        #endregion

        #region Functions
        private async void NewTicketClick()
        {
            ICommand ticketCmd = new Command((newTicket) =>
            {
                Monitor.Enter(this);
                if (newTicket is Ticket)
                    Tickets.Add((Ticket)newTicket);
                Monitor.Exit(this);
            });
            await Shell.Current.Navigation.PushModalAsync(new NewTicketPage((Command)ticketCmd), true);

            // DONT: send ticket back to service
        }
        public void TicketToggled(Ticket ticket)
        {
            bool response = Service.HandleTicket(ticket);
            if (response)
            {
                if (ticket.IsActive == true)
                {
                    ticket.ActiveTimeSpan = TimeSpan.FromHours(24);
                    ticket.ServerActiveTime = $"{(ticket.ActiveTimeSpan.Days * 24 + ticket.ActiveTimeSpan.Hours).ToString("00")}:{ticket.ActiveTimeSpan.Minutes.ToString("00")}:{ticket.ActiveTimeSpan.Seconds.ToString("00")}";
                }
                else if (ticket.IsActive == false)
                {
                    ticket.ActiveTimeSpan = TimeSpan.Zero;
                    ticket.ServerActiveTime = "";
                }
            }
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
