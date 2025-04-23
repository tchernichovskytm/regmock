using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regmock.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
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

        #region Constructor
        public ShellViewModel()
        {
            IsLoggedIn = false;
        }
        #endregion
    }
}
