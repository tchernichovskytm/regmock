using regmock.Models;
using regmock.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace regmock.Components
{
    public class ContactPopupViewModel : ViewModelBase
    {
        #region Properties
        private ContactPopup contactPopup;

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string subject;
        public string Subject
        {
            get => subject;
            set
            {
                subject = value;
                OnPropertyChanged(nameof(Subject));
            }
        }

        private string topic;
        public string Topic
        {
            get => topic;
            set
            {
                topic = value;
                OnPropertyChanged(nameof(Topic));
            }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }
        #endregion

        #region Commands
        public ICommand CloseCmd { get; set; }
        #endregion

        #region Constructor
        public ContactPopupViewModel(ContactPopup contactPopup, Ticket ticket)
        {
            this.contactPopup = contactPopup;

            CloseCmd = new Command(async () =>
            {
                await CloseClicked();
            });

            Name = ticket.Sender.Fullname;
            Subject = ticket.Subject.Name;
            Topic = ticket.Topics.Last();
            PhoneNumber = ticket.Sender.PhoneNumber;
        }
        #endregion

        #region Functions
        public async Task CloseClicked()
        {
            await contactPopup.CloseAsync();
        }
        #endregion
    }
}
