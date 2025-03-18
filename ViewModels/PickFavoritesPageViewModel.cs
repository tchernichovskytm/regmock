using Microsoft.Maui.Animations;
using regmock.Models;
using System.Windows.Input;

using regmock.Views;

namespace regmock.ViewModels
{
    public class PickFavoritesPageViewModel : ViewModelBase
    {
        #region Properties

        private List<Favorite> helperFavorites;

        public List<Favorite> HelperFavorites
        {
            get { return helperFavorites; }
            set
            {
                helperFavorites = value;
                OnPropertyChanged(nameof(helperFavorites));
            }
        }


        #endregion

        #region Commands
       public ICommand AddPreferenceCmd { get; set; }
        #endregion

        #region Constructor
        public PickFavoritesPageViewModel()
        {
            HelperFavorites = Service.GetFavorites();

            AddPreferenceCmd = new Command(AddPreference);
        }
        #endregion

        #region Functions
        private async void AddPreference()
        {
            ICommand preferenceCmd = new Command((newPreference) =>
            {
                if (newPreference is Favorite)
                    HelperFavorites.Add((Favorite)newPreference);
            });
            await Shell.Current.Navigation.PushAsync(new NewPreferencePage((Command)preferenceCmd), true);
        }
        #endregion
    }
}
