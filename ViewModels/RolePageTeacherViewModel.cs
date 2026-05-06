using regmock.Models;
using regmock.Views;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class RolePageTeacherViewModel : ViewModelBase
    {
        #region Properties

        private List<School> schoolList;
        public List<School> SchoolList
        {
            get { return schoolList; }
            set
            {
                schoolList = value;
                OnPropertyChanged(nameof(SchoolList));
            }
        }

        private int schoolSelectIndex;
        public int SchoolSelectIndex
        {
            get { return schoolSelectIndex; }
            set
            {
                schoolSelectIndex = value;
                OnPropertyChanged(nameof(SchoolSelectIndex));
                CheckCanAssign();
            }
        }

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
        #endregion

        #region Commands
        public ICommand CloseCmd { get; set; }
        public ICommand AssignClickCmd { get; set; }
        #endregion

        #region Constructor

        public RolePageTeacherViewModel()
        {
            CloseCmd = new Command(CloseClick);

            SchoolList = Service.GetSchools();
            //GetFullNameSchoolList();
            SchoolSelectIndex = -1;

            AssignClickCmd = new Command(async () =>
            {
                Service.TeacherRegister(SchoolList[SchoolSelectIndex]);

                await Shell.Current.Navigation.PushModalAsync(new RegisterPage(), true);
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
            if (SchoolSelectIndex >= 0)
            {
                CanAssign = true;
            }
        }

        private void GetFullNameSchoolList()
        {
            List<School> tmp = Service.GetSchools();
            SchoolList = new List<School>();
            foreach (School s in tmp)
            {
                School school = new School() { Name = s.Name + " (" + s.City + ")", Id = s.Id, City = s.City };
                SchoolList.Add(school);
            }
        }
        #endregion
    }
}
