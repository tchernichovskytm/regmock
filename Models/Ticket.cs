using System.ComponentModel;
namespace regmock.Models
{
    public class Ticket : INotifyPropertyChanged
    {
        public List<DateTime>? OpenTimes { get; set; }
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
                }
            }
        }

        // Backend properties
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
        public TimeSpan ActiveTimeSpan { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
