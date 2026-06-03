using regmock.Models;
using System.Collections.ObjectModel;
using regmock.Helper;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui.Animations;
using regmock.Components;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Maui.Extensions;

namespace regmock.ViewModels
{
    public class GiveHelpPageViewModel : ViewModelBase
    {
        #region Properties
        private const string FavFalseIcon = IconFont.Favorite_outline;
        private const string FavTrueIcon = IconFont.Favorite;

        private List<Ticket> AllTickets;

        private ObservableCollection<Ticket> displayTickets;

        public ObservableCollection<Ticket> DisplayTickets
        {
            get { return displayTickets; }
            set
            {
                displayTickets = value;
                OnPropertyChanged(nameof(DisplayTickets));
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

        private bool isFavButtonVisible;

        public bool IsFavButtonVisible
        {
            get { return isFavButtonVisible; }
            set
            {
                isFavButtonVisible = value;
                OnPropertyChanged(nameof(IsFavButtonVisible));
            }
        }

        private List<Favorite> HelperFavorites;

        private Ticket selectedTicket;
        public Ticket SelectedTicket
        {
            get => selectedTicket;
            set
            {
                selectedTicket = value;
                OnPropertyChanged(nameof(SelectedTicket));
            }
        }

        private bool isContactPopupVisible;
        public bool IsContactPopupVisible
        {
            get => isContactPopupVisible;
            set
            {
                isContactPopupVisible = value;
                OnPropertyChanged(nameof(IsContactPopupVisible));
            }
        }
        #endregion

        #region Commands
        public ICommand FavButtonCmd { get; set; }
        public ICommand ContactCmd { get; set; }
        public ICommand CloseContactCmd { get; set; }
        public ICommand CopyPhoneNumberCmd { get; set; }
        #endregion

        #region Constructor
        public async Task FetchTicketsAsync()
        {
            if (DisplayTickets != null)
            {
                DisplayTickets.Clear();
            }
            // get all the items from the fb into the service
            await Service.GetAllTicketsFromFB();
            // get the new ticket list from the service
            AllTickets = Service.GetOthersTickets();
            BringTopicsToFirst(AllTickets);

            DisplayTicketsByFavorite();

            await Service.GetHelperFavoritesFromFB();
            HelperFavorites = Service.GetHelperFavorites();
            IsFavButtonVisible = HelperFavorites.Count > 0 && PutTicketFavoriteIcons(AllTickets) > 0;
        }

        public GiveHelpPageViewModel()
        {
            FavButtonIcon = FavFalseIcon;
            IsFav = false;
            FavButtonCmd = new Command(FavButtonClick);
            ContactCmd = new Command((object obj) =>
            {
                if (obj is Ticket ticket)
                {
                    SelectedTicket = ticket;
                    IsContactPopupVisible = true;
                }
            });
            CloseContactCmd = new Command(() =>
            {
                IsContactPopupVisible = false;
                SelectedTicket = null;
            });

            CopyPhoneNumberCmd = new Command(async () =>
            {
                await Clipboard.Default.SetTextAsync(SelectedTicket.Sender.PhoneNumber);
            });
        }
        #endregion

        #region Functions
        public void FavButtonClick()
        {
            IsFav = !IsFav;
            FavButtonIcon = IsFav ? FavTrueIcon : FavFalseIcon;
            DisplayTicketsByFavorite();
        }

        public void DisplayTicketsByFavorite()
        {
            if (IsFav == true)
            {
                DisplayTickets = FilterTicketsByFavorites(AllTickets, HelperFavorites);
            }
            else
            {
                DisplayTickets = new ObservableCollection<Ticket>(AllTickets);
            }
        }

        public bool PassesFilter(Ticket ticket, List<Favorite> helperFavorites)
        {
            foreach (Favorite favorite in helperFavorites)
            {
                if (ticket.Subject.Id == favorite.Subject.Id)
                {
                    foreach (Grade grade in favorite.Grades)
                    {
                        if (ticket.Sender.Grade.Id == grade.Id)
                        {
                            return true;
                        }
                    }
                    break;
                }
            }
            return false;
        }

        public ObservableCollection<Ticket> FilterTicketsByFavorites(List<Ticket> tickets, List<Favorite> helperFavorites)
        {
            ObservableCollection<Ticket> filtered = new ObservableCollection<Ticket>();
            foreach (Ticket ticket in tickets)
            {
                if (PassesFilter(ticket, helperFavorites)) filtered.Add(ticket);
            }
            return filtered;
        }

        public void BringTopicsToFirst(List<Ticket> tickets)
        {
            foreach (Ticket ticket in tickets)
            {
                ticket.Topics = new List<string> { ticket.Topics.LastOrDefault() };
            }
        }

        public int PutTicketFavoriteIcons(List<Ticket> tickets)
        {
            int passesFilterCount = 0;
            foreach (Ticket ticket in tickets)
            {
                bool passes = PassesFilter(ticket, HelperFavorites);
                ticket.IsFavoriteIcon = passes ? FavTrueIcon : "";
                if (passes) passesFilterCount++;

            }
            return passesFilterCount;
        }
        #endregion
    }
}
