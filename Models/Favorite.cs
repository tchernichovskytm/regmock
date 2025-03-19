using System.ComponentModel;

namespace regmock.Models
{
    public class Favorite : INotifyPropertyChanged
    {
        private Subject? subject;
        public Subject? Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        private List<Grade>? grades;
        public List<Grade>? Grades
        {
            get
            {
                return grades;
            }
            set
            {
                grades = value;
                OnPropertyChanged(nameof(Grades));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
