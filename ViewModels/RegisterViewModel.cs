using regmock.Models;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        #region Properties
        private bool canRegister;

        public bool CanRegister
        {
            get { return canRegister; }
            set
            {
                canRegister = value;
                OnPropertyChanged(nameof(CanRegister));
            }
        }

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

        private string? fullnameEntry;

        public string? FullnameEntry
        {
            get { return fullnameEntry; }
            set
            {
                fullnameEntry = value;
                checkValidRegister();
                OnPropertyChanged(nameof(FullnameEntry));
            }
        }

        private string? fullnameErr;

        public string? FullnameErr
        {
            get { return fullnameErr; }
            set
            {
                fullnameErr = value;
                OnPropertyChanged(nameof(FullnameErr));
            }
        }

        private string? emailEntry;

        public string? EmailEntry
        {
            get { return emailEntry; }
            set
            {
                emailEntry = value;
                checkValidRegister();
                OnPropertyChanged(nameof(EmailEntry));
            }
        }

        private string? emailErr;

        public string? EmailErr
        {
            get { return emailErr; }
            set
            {
                emailErr = value;
                OnPropertyChanged(nameof(EmailErr));
            }
        }

        private string? passwordEntry;

        public string? PasswordEntry
        {
            get { return passwordEntry; }
            set
            {
                passwordEntry = value;
                checkValidRegister();
                OnPropertyChanged(nameof(PasswordEntry));
            }
        }

        private string? passwordErr;

        public string? PasswordErr
        {
            get { return passwordErr; }
            set
            {
                passwordErr = value;
                OnPropertyChanged(nameof(PasswordErr));
            }
        }

        private string? phonenumberEntry;

        public string? PhonenumberEntry
        {
            get { return phonenumberEntry; }
            set
            {
                phonenumberEntry = value;
                checkValidRegister();
                OnPropertyChanged(nameof(PhonenumberEntry));
            }
        }

        private string? phonenumberErr;

        public string? PhonenumberErr
        {
            get { return phonenumberErr; }
            set
            {
                phonenumberErr = value;
                OnPropertyChanged(nameof(PhonenumberErr));
            }
        }

        private List<Grade> gradeList;

        public List<Grade> GradeList
        {
            get { return gradeList; }
            set
            {
                gradeList = value;
                OnPropertyChanged(nameof(GradeList));
            }
        }

        private string? registerErr;

        public string? RegisterErr
        {
            get { return registerErr; }
            set { registerErr = value; OnPropertyChanged(nameof(RegisterErr)); }
        }

        #endregion

        #region Commands
        public ICommand FullnameReset_Cmd { get; set; }
        public ICommand EmailReset_Cmd { get; set; }
        public ICommand PassReset_Cmd { get; set; }
        public ICommand PhoneReset_Cmd { get; set; }
        public ICommand ShowPass_Cmd { get; set; }
        public ICommand Register_Cmd { get; set; }
        #endregion

        #region Constructor
        public RegisterViewModel()
        {
            //fullnameEntry = "";
            //emailEntry = "";
            //passwordEntry = "";
            //phonenumberEntry = "";

            //fullnameErr = "";
            //emailErr = "";
            //passwordErr = "";
            //phonenumberErr = "";

            CanRegister = false;
            IsPassVisible = true;

            ShowPass_Cmd = new Command(ShowPass);
            FullnameReset_Cmd = new Command(ResetFullname);
            EmailReset_Cmd = new Command(ResetEmail);
            PassReset_Cmd = new Command(ResetPass);
            PhoneReset_Cmd = new Command(ResetPhone);
            Register_Cmd = new Command(() => {
                bool success = RegisterClicked();
                if (success)
                {
                    RegisterErr = "Success";
                }
                else
                {
                    RegisterErr = "Failed to Register";
                }
            });

            GradeList = Service.GetGrades();
        }
        #endregion

        #region Functions
        private bool RegisterClicked()
        {
            bool success = Service.RequestRegister(FullnameEntry, PhonenumberEntry, EmailEntry, PasswordEntry);
            return success;
        }
        private void ResetFullname()
        {
            FullnameEntry = "";
        }
        private void ResetEmail()
        {
            EmailEntry = "";
        }
        private void ResetPass()
        {
            PasswordEntry = "";
        }
        private void ResetPhone()
        {
            PhonenumberEntry = "";
        }
        private void ShowPass()
        {
            IsPassVisible = !IsPassVisible;
        }

        private void checkValidRegister()
        {
            bool validFullname = false;
            if (FullnameEntry == null) FullnameErr = "";
            else if (FullnameEntry.Length == 0) FullnameErr = "Please enter a user name";
            else if (FullnameEntry.Length < 3) FullnameErr = "Too short";
            else if (FullnameEntry.Length > 12) FullnameErr = "Too long";
            else { FullnameErr = ""; validFullname = true; }

            bool validPhone = false;
            Regex validatePhoneRegex = new Regex("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");
            if (PhonenumberEntry != null)
            {
                validPhone = validatePhoneRegex.IsMatch(PhonenumberEntry);
                if (PhonenumberEntry.Length == 0) PhonenumberErr = "Please enter a valid phone number";
                else if (!validPhone) PhonenumberErr = "Phone number not legal";
                else { PhonenumberErr = ""; }
            }

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
                else if (!validRegexCapital) { PasswordErr = "Password must have capital letters"; }
                else if (!validRegexLower) { PasswordErr = "Password must have lowercase letters"; }
                else if (!validRegexDigits) { PasswordErr = "Password must have digits"; }
                else { PasswordErr = ""; validPassword = true; }
            }

            CanRegister = validFullname && validPhone && validEmail && validPassword;
        }
        #endregion
    }
}
