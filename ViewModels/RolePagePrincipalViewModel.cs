using regmock.Models;
using regmock.Views;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class RolePagePrincipalViewModel : ViewModelBase
    {
        #region Properties
        private bool canAssign;
        public bool CanAssign
        {
            get { return canAssign; }
            set
            {
                canAssign = value;
                OnPropertyChanged(nameof(CanAssign));
            }
        }

        private string schoolEntry;

        public string SchoolEntry
        {
            get { return schoolEntry; }
            set
            {
                schoolEntry = value;
                OnPropertyChanged(nameof(SchoolEntry));
                CheckCanAssign();
            }
        }

        #endregion

        #region Commands
        public ICommand CloseCmd { get; set; }
        public ICommand AssignClickCmd { get; set; }
        #endregion

        #region Constructor
        public RolePagePrincipalViewModel()
        {
            CloseCmd = new Command(CloseClick);

            AssignClickCmd = new Command(() =>
            {
                // TODO: send to page that says "request pending" or somrthin g
                //await Shell.Current.Navigation.PushModalAsync(new RegisterPage(), true);
            });
        }
        #endregion

        #region Functions
        public async void CloseClick()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }

        public void CheckCanAssign()
        {
            // TODO: a regex can be added for verification
            if (!string.IsNullOrEmpty(SchoolEntry))
            {
                CanAssign = true;
            }
        }
        #endregion
    }
}
