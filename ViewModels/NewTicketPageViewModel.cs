using regmock.Models;
using regmock.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class NewTicketPageViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<Subject> subjects;

        public ObservableCollection<Subject> Subjects
        {
            get { return subjects; }
            set
            {
                subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        private string topicEntry;

        public string TopicEntry
        {
            get { return topicEntry; }
            set
            {
                topicEntry = value;
                OnPropertyChanged(nameof(TopicEntry));
                CheckValidTicket();
            }
        }

        private string topicErr;

        public string TopicErr
        {
            get { return topicErr; }
            set
            {
                topicErr = value;
                OnPropertyChanged(nameof(TopicErr));
            }
        }

        private bool canAdd;

        public bool CanAdd
        {
            get { return canAdd; }
            set
            {
                canAdd = value;
                OnPropertyChanged(nameof(CanAdd));
            }
        }

        private int selectedSubjectIndex;

        public int SelectedSubjectIndex
        {
            get { return selectedSubjectIndex; }
            set
            {
                selectedSubjectIndex = value;
                OnPropertyChanged(nameof(SelectedSubjectIndex));
            }
        }

        #endregion
        #region Commands
        public ICommand CloseCmd { get; set; }
        public ICommand AddTicketCmd { get; set; }
        public ICommand TicketCmd { get; set; }
        #endregion
        #region Constructor
        public NewTicketPageViewModel(Command ticketCmd)
        {
            Subjects = new ObservableCollection<Subject>(Service.GetSubjects());

            TopicErr = "";
            CanAdd = false;

            SelectedSubjectIndex = 0;

            CloseCmd = new Command(CloseClick);
            AddTicketCmd = new Command(AddTicketClick);
            TicketCmd = ticketCmd;
        }
        #endregion
        #region Functions
        public async void CloseClick()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }
        public async void AddTicketClick()
        {
            Ticket NewTicket = new Ticket()
            {
                Subject = Subjects[selectedSubjectIndex],
                Topics = new List<string>() { TopicEntry },
                IsActive = false,
            };

            if (Service.AddTicket(NewTicket))
            {
                TicketCmd.Execute(NewTicket);
                await Shell.Current.Navigation.PopModalAsync(true);
            }
            else
            {
                // TODO: handle error
            }
        }

        public void CheckValidTicket()
        {
            bool validTopicEntry = false;
            if (TopicEntry == null) TopicErr = "";
            else if (TopicEntry.Length == 0) TopicErr = "Please enter a topic";
            else { TopicErr = ""; validTopicEntry = true; }

            if (validTopicEntry)
            {
                CanAdd = true;
            }
            else
            {
                CanAdd = false;
            }
        }
        #endregion
    }
}
