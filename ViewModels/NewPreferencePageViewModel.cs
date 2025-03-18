using regmock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class NewPreferencePageViewModel : ViewModelBase
    {
        #region Properties
        #endregion
        #region Commands
        public ICommand CloseCmd { get; set; }
        public ICommand AddPreferenceCmd { get; set; }
        public ICommand PreferenceCmd { get; set; }
        #endregion

        #region Constructor
        public NewPreferencePageViewModel(Command preferenceCmd)
        {
            PreferenceCmd = preferenceCmd;

            CloseCmd = new Command(CloseClick);
        }
        #endregion

        #region Functions
        private async void CloseClick()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }
        private async void AddPreference()
        {
            Favorite NewFavorite = new Favorite()
            {
                
            };

            if (Service.AddFavorite(NewFavorite))
            {
                PreferenceCmd.Execute(NewFavorite);
                await Shell.Current.Navigation.PopModalAsync(true);
            }
            else
            {
                // TODO: handle error
            }
        }
        #endregion
    }
}
