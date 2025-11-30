using System.ComponentModel;
using System.Windows.Input;
namespace regmock.Models
{
    public class Ticket : INotifyPropertyChanged
    {
        public List<Int64>? OpenTimes { get; set; }
        public Subject? Subject { get; set; }
        public List<string>? Topics { get; set; }
        public User? Sender { get; set; }
        public List<User>? Helpers { get; set; }

        private bool? isActive;
        public bool? IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                    if (IsActiveToggleCmd != null && IsActiveToggleCmd.CanExecute(this) == true)
                    {
                        IsActiveToggleCmd.Execute(this);
                    }
                }
            }
        }

        public string? FirebaseKey { get; set; }

        #region BackendProperties
        public ICommand? IsActiveToggleCmd { get; set; }
        public ICommand? DeleteCmd { get; set; }

        private string? serverActiveTime;
        public string? ServerActiveTime
        {
            get { return serverActiveTime; }
            set
            {
                if (serverActiveTime != value)
                {
                    serverActiveTime = value;
                    OnPropertyChanged(nameof(ServerActiveTime));
                }
            }
        }
        public Int64 ActiveTimeSpan { get; set; }

        private string isFavoriteIcon;

        public string IsFavoriteIcon
        {
            get { return isFavoriteIcon; }
            set
            {
                isFavoriteIcon = value;
                OnPropertyChanged(nameof(IsFavoriteIcon));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
