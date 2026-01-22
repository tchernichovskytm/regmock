using regmock.Models;
using regmock.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class NewFavoritePageViewModel : ViewModelBase
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

        private ObservableCollection<ToggleGrade> toggleGrades;

        public ObservableCollection<ToggleGrade> ToggleGrades
        {
            get { return toggleGrades; }
            set
            {
                toggleGrades = value;
                OnPropertyChanged(nameof(ToggleGrades));
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
        #endregion

        #region Commands
        public ICommand GradeToggleCmd { get; set; }
        public ICommand CloseCmd { get; set; }
        public ICommand AddFavoriteCmd { get; set; }
        public ICommand FavoriteCmd { get; set; }
        #endregion

        #region Constructor
        public NewFavoritePageViewModel(List<Subject> existingSubjects, Command favoriteCmd)
        {
            Subjects = new ObservableCollection<Subject>(Service.GetSubjects());

            foreach (Subject existingSubject in existingSubjects)
            {
                if (Subjects.Contains(existingSubject))
                {
                    Subjects.Remove(existingSubject);
                }
            }

            ToggleGrades = new ObservableCollection<ToggleGrade>();
            List<Grade> Grades = Service.GetGrades();

            GradeToggleCmd = new Command(GradeToggleClick);
            foreach (Grade grade in Grades)
            {
                ToggleGrade newTG = new ToggleGrade();
                newTG.Grade = grade;
                newTG.Toggled = false;
                newTG.OnToggleChangedCommand = (Command)GradeToggleCmd;
                ToggleGrades.Add(newTG);
            }

            SelectedSubjectIndex = 0;
            CanAdd = false;

            FavoriteCmd = favoriteCmd;

            CloseCmd = new Command(CloseClick);
            AddFavoriteCmd = new Command(AddFavorite);

        }
        #endregion

        #region Functions
        private void GradeToggleClick()
        {
            foreach (ToggleGrade tg in ToggleGrades)
            {
                if (tg.Toggled == true)
                {
                    CanAdd = true;
                    return;
                }
            }
            CanAdd = false;
        }
        private async void CloseClick()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }
        private async void AddFavorite()
        {
            List<Grade> toggledGrades = new List<Grade>();
            foreach (ToggleGrade tg in ToggleGrades)
            {
                if (tg.Toggled == true)
                {
                    toggledGrades.Add(tg.Grade);
                }
            }

            Favorite NewFavorite = new Favorite()
            {
                Subject = Subjects[selectedSubjectIndex],
                Grades = toggledGrades,
            };

            //var success = await Service.AddFavorite(NewFavorite);
            //if (success)
            //{
            FavoriteCmd.Execute(NewFavorite);
            await Shell.Current.Navigation.PopModalAsync(true);
            //}
            //else
            //{
            // TODO: handle error
            //}
        }
        #endregion
    }
}
