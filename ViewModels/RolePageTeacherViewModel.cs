using regmock.Models;

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

        #endregion

        #region Commands
        #endregion

        #region Constructor

        public RolePageTeacherViewModel()
        {
            GetFullNameSchoolList();
            SchoolErr = string.Empty;

        }

        #endregion

        #region Functions
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
