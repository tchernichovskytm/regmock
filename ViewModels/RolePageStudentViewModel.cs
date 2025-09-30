using regmock.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regmock.ViewModels
{
    public class RolePageStudentViewModel : ViewModelBase
    {
        #region Properties
        //private string schoolEntry;

        //public string SchoolEntry
        //{
        //    get { return schoolEntry; }
        //    set
        //    {
        //        schoolEntry = value;
        //        OnPropertyChanged(nameof(SchoolEntry));
        //    }
        //}
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


        private string schoolErr;

        public string SchoolErr
        {
            get { return schoolErr; }
            set
            {
                schoolErr = value;
                OnPropertyChanged(nameof(SchoolErr));
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

        private string gradeErr;

        public string GradeErr
        {
            get { return gradeErr; }
            set
            {
                gradeErr = value;
                OnPropertyChanged(nameof(GradeErr));
            }
        }

        #endregion

        #region Commands
        #endregion

        #region Constructor

        public RolePageStudentViewModel()
        {
            //SchoolEntry = "";
            GetFullNameSchoolList();
            SchoolErr = "";

            GradeList = Service.GetGrades();
            GradeErr = "";
        }

        #endregion

        #region Functions
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
