using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region Properties
        private bool isLoggedIn;

        public bool IsNotLoggedIn
        {
            get { return !isLoggedIn; }
        }

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
                OnPropertyChanged(nameof(IsNotLoggedIn));
            }
        }
        #endregion

        #region Commands
        public ICommand DarkModeCmd { get; set; }

        // these control the visibility of buttons
        public ICommand LoggedInCmd { get; set; }
        public ICommand LoggedOutCmd { get; set; }

        // this is a request to log out
        public ICommand LogOutCmd { get; set; }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            DarkModeCmd = new Command(() =>
            {
                // toggle dark and light
                Application.Current.UserAppTheme =
                Application.Current.UserAppTheme == AppTheme.Light ?
                                                    AppTheme.Dark :
                                                    AppTheme.Light;
            });
            LogOutCmd = new Command(async () =>
            {
                if (!Service.RequestLogoutAsync())
                {
                    // TODO: handle error
                }
                LoggedOutCmd.Execute(null);
                await Shell.Current.GoToAsync("//LoginPage");
            });

            IsLoggedIn = false;

            LoggedInCmd = new Command(() =>
            {
                IsLoggedIn = true;
            });
            LoggedOutCmd = new Command(() =>
            {
                IsLoggedIn = false;
            });

            Service.LoggedInCommand = LoggedInCmd;
            Service.LoggedOutCommand = LoggedOutCmd;
        }
        #endregion
    }
}
