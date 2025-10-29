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
        public ICommand DarkMode_Cmd { get; set; }
        public ICommand LoggedInCmd { get; set; }
        public ICommand LoggedOutCmd { get; set; }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            DarkMode_Cmd = new Command(() =>
            {
                // toggle dark and light
                Application.Current.UserAppTheme =
                Application.Current.UserAppTheme == AppTheme.Light ?
                                                    AppTheme.Dark :
                                                    AppTheme.Light;
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
