using regmock.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace regmock.ViewModels
{
    public class RolePageViewModel : ViewModelBase
    {
        #region Properties
        #endregion

        #region Commands
        public ICommand StudentClick_Cmd { get; set; }
        public ICommand TeacherClick_Cmd { get; set; }
        public ICommand PrincipalClick_Cmd { get; set; }
        #endregion

        #region Constructor
        public RolePageViewModel()
        {
            StudentClick_Cmd = new Command(async () => {
                await Shell.Current.Navigation.PushModalAsync(new RolePageStudent(), true);
            });
            TeacherClick_Cmd = new Command(async () => {
                await Shell.Current.Navigation.PushModalAsync(new RolePageTeacher(), true);
            });
            PrincipalClick_Cmd = new Command(async () => {
                await Shell.Current.Navigation.PushModalAsync(new RolePagePrincipal(), true);
            });
        }
        #endregion

        #region Functions
        #endregion
    }
}
