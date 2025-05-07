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
        private bool isNotLoggedIn;

        public bool IsNotLoggedIn
        {
            get { return isNotLoggedIn; }
            set
            {
                isNotLoggedIn = value;
                OnPropertyChanged(nameof(IsNotLoggedIn));
            }
        }

        private bool isLoggedIn;

        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                IsNotLoggedIn = !value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }
        #endregion

        #region Commands
        public ICommand LoggedInCmd { get; set; }
        public ICommand LoggedOutCmd { get; set; }
        #endregion

        #region Constructor
        public ShellViewModel()
        {
            IsLoggedIn = false;

            LoggedInCmd = new Command(() =>
            {
                IsLoggedIn = true;
            });
            LoggedOutCmd = new Command(() =>
            {
                IsLoggedIn = false;
            });
        }
        #endregion
    }
}
