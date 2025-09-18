using regmock.Models;
using regmock.ViewModels;

namespace regmock.Views;

public partial class NewPreferencePage : ContentPage
{
    public NewPreferencePage(List<Subject> ExistingSubjects, Command preferenceCmd)
    {
        InitializeComponent();

        BindingContext = new NewPreferencePageViewModel(ExistingSubjects, preferenceCmd);
    }
}