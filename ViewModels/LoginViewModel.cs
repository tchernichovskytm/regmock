using System.Windows.Input;
using System.Text.RegularExpressions;

namespace regmock.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region Properties
        private bool isPassVisible;

        public bool IsPassVisible
        {
            get { return isPassVisible; }
            set
            {
                isPassVisible = value;
                OnPropertyChanged(nameof(IsPassVisible));
            }
        }


        private string passwordEntry;
        public string PasswordEntry
        {
            get { return passwordEntry; }
            set
            {
                passwordEntry = value;
                checkValidLogin();
                OnPropertyChanged(nameof(PasswordEntry));
            }
        }
        private string passwordErr;
        public string PasswordErr
        {
            get { return passwordErr; }
            set
            {
                passwordErr = value;
                OnPropertyChanged(nameof(PasswordErr));
            }
        }

        private string usernameEntry;
        public string UsernameEntry
        {
            get { return usernameEntry; }
            set
            {
                usernameEntry = value;
                checkValidLogin();
                OnPropertyChanged(nameof(UsernameEntry));
            }
        }

        private string usernameErr;
        public string UsernameErr
        {
            get { return usernameErr; }
            set
            {
                usernameErr = value;
                OnPropertyChanged(nameof(UsernameErr));
            }
        }

        #endregion

        #region Commands
        public ICommand ShowPass_Cmd { get; set; }
        public ICommand PassReset_Cmd { get; set; }
        public ICommand UserReset_Cmd { get; set; }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            usernameErr = "";
            passwordErr = "";

            IsPassVisible = true;

            ShowPass_Cmd = new Command(ShowPass);
            PassReset_Cmd = new Command(ResetPass);
            UserReset_Cmd = new Command(ResetUser);
        }
        #endregion

        #region Functions
        private void ResetUser()
        {
            UsernameEntry = "";
        }

        private void ResetPass()
        {
            PasswordEntry = "";
        }

        private void ShowPass()
        {
            IsPassVisible = !IsPassVisible;
        }

        private void checkValidLogin()
        {
            bool validUsername = false;
            if (UsernameEntry == null) UsernameErr = "";
            else if (UsernameEntry.Length == 0) UsernameErr = "Please enter a user name";
            else if (UsernameEntry.Length < 3) UsernameErr = "Too short";
            else if (UsernameEntry.Length > 12) UsernameErr = "Too long";
            else { UsernameErr = ""; validUsername = true; }

            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            bool validPassword = false;
            if (PasswordEntry != null)
            {
                validPassword = validateGuidRegex.IsMatch(PasswordEntry);
                if (PasswordEntry.Length == 0) PasswordErr = "Please enter a password";
                else if (!validPassword) PasswordErr = "Password not legal";
                else { PasswordErr = ""; }
            }

            if (validUsername && validPassword)
            {
                // TODO: valid login, check if user exists and log in
                bool success = Service.RequestLogin(UsernameEntry, PasswordEntry);
                if (success)
                {

                }
                else
                {

                }
            }
        }
        #endregion
    }
}
