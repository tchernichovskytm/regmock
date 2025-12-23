using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Maui.Graphics.Text;

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

        private string emailEntry;
        public string EmailEntry
        {
            get { return emailEntry; }
            set
            {
                emailEntry = value;
                checkValidLogin();
                OnPropertyChanged(nameof(EmailEntry));
            }
        }

        private string emailErr;
        public string EmailErr
        {
            get { return emailErr; }
            set
            {
                emailErr = value;
                OnPropertyChanged(nameof(EmailErr));
            }
        }

        private bool canLogin;

        public bool CanLogin
        {
            get { return canLogin; }
            set
            {
                canLogin = value;
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        private string loginErr;

        public string LoginErr
        {
            get { return loginErr; }
            set
            {
                loginErr = value;
                OnPropertyChanged(nameof(LoginErr));
            }
        }

        private Color loginErrColor;

        public Color LoginErrColor
        {
            get { return loginErrColor; }
            set
            {
                loginErrColor = value;
                OnPropertyChanged(nameof(LoginErrColor));
            }
        }


        #endregion

        #region Commands
        public ICommand ShowPass_Cmd { get; set; }
        public ICommand PassReset_Cmd { get; set; }
        public ICommand EmailReset_Cmd { get; set; }
        public ICommand Login_Cmd { get; set; }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            emailErr = "";
            passwordErr = "";
            loginErr = "";

            IsPassVisible = true;

            ShowPass_Cmd = new Command(ShowPass);
            PassReset_Cmd = new Command(ResetPass);
            EmailReset_Cmd = new Command(ResetEmail);

            Login_Cmd = new Command(async () =>
            {
                bool success = await Service.RequestLoginAsync(EmailEntry, PasswordEntry);
                if (success)
                {
                    LoginErr = "Logged in";
                    LoginErrColor = Colors.Green;

                    // TODO: make this go to a real page later

                    await Shell.Current.GoToAsync("//GiveHelpPage");
                    //await Shell.Current.GoToAsync("//RequestHelpPage");
                    //await Shell.Current.GoToAsync("//PickFavoritesPage");
                }
                else
                {
                    LoginErr = "Failed to Login, Please try again";
                    LoginErrColor = Colors.Red;
                }
            });
        }
        #endregion

        #region Functions
        private void ResetEmail()
        {
            EmailEntry = "";
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
            // TODO: remove this hardcoded login
            //if ((emailEntry == "idosweed121@gmail.com" || emailEntry == "info@eldanet.com") && passwordEntry == "123456")
            //{
            //    CanLogin = true;
            //    LoginErr = "";
            //    return;
            //}

            // TODO: verify email regex
            bool validEmail = false;
            if (EmailEntry != null)
            {
                Regex validateEmailRegex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
                bool validRegex = validateEmailRegex.IsMatch(EmailEntry);
                if (EmailEntry.Length == 0) EmailErr = "Please enter an email";
                else if (!validRegex) EmailErr = "Invalid Email";
                else { EmailErr = ""; validEmail = true; }
            }

            bool validPassword = false;
            if (PasswordEntry != null)
            {
                // Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
                Regex validateGuidRegexCapital = new Regex("^(?=.*?[A-Z]).{1,}$");
                bool validRegexCapital = validateGuidRegexCapital.IsMatch(PasswordEntry);

                Regex validateGuidRegexLower = new Regex("^(?=.*?[a-z]).{1,}$");
                bool validRegexLower = validateGuidRegexLower.IsMatch(PasswordEntry);

                Regex validateGuidRegexDigits = new Regex("^(?=.*?[0-9]).{1,}$");
                bool validRegexDigits = validateGuidRegexDigits.IsMatch(PasswordEntry);

                if (PasswordEntry.Length == 0) PasswordErr = "Please enter a password";
                else if (PasswordEntry.Length < 6) PasswordErr = "Password must be at least 6 characters";
                //else if (!validRegexCapital) { PasswordErr = "Password must have capital letters"; }
                //else if (!validRegexLower) { PasswordErr = "Password must have lowercase letters"; }
                //else if (!validRegexDigits) { PasswordErr = "Password must have digits"; }
                else { PasswordErr = ""; validPassword = true; }
            }

            CanLogin = validEmail && validPassword;
            LoginErr = "";
        }
        #endregion
    }
}
