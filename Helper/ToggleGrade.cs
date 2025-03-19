using regmock.Models;

namespace regmock.Helper
{
    public class ToggleGrade
    {
        public Command? OnToggleChangedCommand { get; set; }
        public Grade? Grade { get; set; }

        private bool toggled;
        public bool Toggled
        {
            get
            {
                return toggled;
            }
            set
            {
                toggled = value;
                if (OnToggleChangedCommand != null && OnToggleChangedCommand.CanExecute(null) == true)
                {
                    OnToggleChangedCommand.Execute(null);
                }
            }
        }

        public ToggleGrade()
        {
            Grade = null;
            toggled = false;
            OnToggleChangedCommand = null;
        }
    }
}
