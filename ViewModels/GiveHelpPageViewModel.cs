using regmock.Models;
using System.Collections.ObjectModel;
using regmock.Helper;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class GiveHelpPageViewModel : ViewModelBase
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

        private string favButtonIcon;

        public string FavButtonIcon
        {
            get { return favButtonIcon; }
            set
            {
                favButtonIcon = value;
                OnPropertyChanged(nameof(FavButtonIcon));
            }
        }

        private bool isFav;

        public bool IsFav
        {
            get { return isFav; }
            set
            {
                isFav = value;
                OnPropertyChanged(nameof(IsFav));
            }
        }


        #endregion

        #region Commands
        public ICommand FavButtonCmd { get; set; }
        #endregion

        #region Constructor
        public GiveHelpPageViewModel()
        {
            GetServiceTickets();

            BringTopicsToFirst();

            FavButtonIcon = IconFont.Favorite_outline;
            IsFav = false;
            FavButtonCmd = new Command(FavButtonClick);
        }
        #endregion

        #region Functions
        public async void GetServiceTickets()
        {
            List<Ticket> ticketsList = await Service.GetAllTickets();
            Tickets = new ObservableCollection<Ticket>(ticketsList);
        }
        public void FavButtonClick()
        {
            if (IsFav == false)
            {
                FavButtonIcon = IconFont.Favorite;
                IsFav = true;
            }
            else if (IsFav == true)
            {
                FavButtonIcon = IconFont.Favorite_outline;
                IsFav = false;
            }
        }

        public void BringTopicsToFirst()
        {
            foreach (Ticket ticket in Tickets)
            {
                ticket.Topics = new List<string> { ticket.Topics.LastOrDefault() };
            }
        }
        #endregion
    }
}
