using regmock.Models;
using regmock.Helper;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class EditPreferencePageViewModel : ViewModelBase
    {
        #region Properties
        //private ObservableCollection<Subject> subjects;

        //public ObservableCollection<Subject> Subjects
        //{
        //    get { return subjects; }
        //    set
        //    {
        //        subjects = value;
        //        OnPropertyChanged(nameof(Subjects));
        //    }
        //}

        //private int selectedSubjectIndex;

        //public int SelectedSubjectIndex
        //{
        //    get { return selectedSubjectIndex; }
        //    set
        //    {
        //        selectedSubjectIndex = value;
        //        OnPropertyChanged(nameof(SelectedSubjectIndex));
        //    }
        //}

        private Subject editSubject;


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

        private bool canApply;

        public bool CanApply
        {
            get { return canApply; }
            set
            {
                canApply = value;
                OnPropertyChanged(nameof(CanApply));
            }
        }

        private Favorite OldFavorite;
        #endregion

        #region Commands
        public ICommand ApplyCmd { get; set; }
        public ICommand DeleteCmd { get; set; }
        public ICommand GradeToggleCmd { get; set; }
        public ICommand CloseCmd { get; set; }
        public ICommand FavoriteCmd { get; set; }
        #endregion

        #region Constructor
        public EditPreferencePageViewModel(Favorite favorite, Command favoriteCmd)
        {
            OldFavorite = favorite;
            //Subjects = new ObservableCollection<Subject>(Service.GetSubjects());
            ToggleGrades = new ObservableCollection<ToggleGrade>();
            List<Grade> Grades = Service.GetGrades();

            GradeToggleCmd = new Command(GradeToggleClick);
            foreach (Grade grade in Grades)
            {
                ToggleGrade newTG = new ToggleGrade();
                newTG.Grade = grade;

                bool found = false;
                foreach (Grade g in favorite.Grades)
                {
                    if (g.Id == grade.Id)
                    {
                        found = true;
                        break;
                    }
                }
                newTG.Toggled = found;

                newTG.OnToggleChangedCommand = (Command)GradeToggleCmd;
                ToggleGrades.Add(newTG);
            }

            editSubject = favorite.Subject;

            //int foundIdx = -1;
            //for (int i = 0; i < Subjects.Count; i++)
            //{
            //    if (Subjects[i].Id == favorite.Subject.Id)
            //    {
            //        foundIdx = i;
            //        break;
            //    }
            //}

            //SelectedSubjectIndex = foundIdx;

            FavoriteCmd = favoriteCmd;

            CloseCmd = new Command(CloseClick);
            ApplyCmd = new Command(ApplyClick);
            DeleteCmd = new Command(DeleteClick);
        }
        #endregion

        #region Functions
        private void GradeToggleClick()
        {
            foreach (ToggleGrade tg in ToggleGrades)
            {
                if (tg.Toggled == true)
                {
                    CanApply = true;
                    return;
                }
            }
            CanApply = false;
        }
        private async void CloseClick()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }
        private async void ApplyClick()
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
                //Subject = Subjects[selectedSubjectIndex],
                Subject = editSubject,
                Grades = toggledGrades,
            };

            //var success = await Service.EditFavorite(OldFavorite, NewFavorite);
            //if (success)
            //{
                FavoriteCmd.Execute(new List<Favorite>() { OldFavorite, NewFavorite });
                await Shell.Current.Navigation.PopModalAsync(true);
            //}
            //else
            //{
                // TODO: handle error
            //}
        }
        private async void DeleteClick()
        {
            //var success = await Service.RemoveFavorite(OldFavorite);
            //if (success)
            //{
                FavoriteCmd.Execute(new List<Favorite>() { OldFavorite, null });
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
