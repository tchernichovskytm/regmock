using regmock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class RolePageStudentViewModel : ViewModelBase
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

        private int gradeSelectIndex;
        public int GradeSelectIndex
        {
            get { return gradeSelectIndex; }
            set
            {
                gradeSelectIndex = value;
                OnPropertyChanged(nameof(GradeSelectIndex));
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
        public ICommand AssignClick_Cmd { get; set; }
        #endregion

        #region Constructor

        public RolePageStudentViewModel()
        {
            SchoolList = Service.GetSchools();
            //GetFullNameSchoolList();
            SchoolSelectIndex = -1;

            GradeList = Service.GetGrades();
            GradeSelectIndex = -1;

            AssignClick_Cmd = new Command(async () =>
            {
                await Service.StudentRegisterAsync(SchoolList[SchoolSelectIndex], GradeList[GradeSelectIndex]);

                // TODO: add a welcome page and go to it
                await Shell.Current.GoToAsync("//RequestHelpPage");
            });
        }

        #endregion

        #region Functions
        public void CheckCanAssign()
        {
            if (SchoolSelectIndex >= 0 && GradeSelectIndex >= 0)
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
                School school = new School() { Name = $"{s.Name} ({s.City})", Id = s.Id, City = s.City };
                SchoolList.Add(school);
            }
        }
        #endregion
    }
}
