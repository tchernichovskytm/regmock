using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regmock.ViewModels
{
    internal class TestingVM : ViewModelBase
    {
        private bool myBool { get; set; }

        public bool MyBool
        {
            get
            {
                return myBool;
            }
            set
            {
                myBool = value;
                OnPropertyChanged(nameof(MyBool));
            }
        }

        public TestingVM()
        {
            MyBool = true;
        }
    }
}
