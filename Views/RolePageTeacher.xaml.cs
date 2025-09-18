namespace regmock.Views;
using regmock.ViewModels;

public partial class RolePageTeacher : ContentPage
{
    public RolePageTeacher()
    {
        InitializeComponent();

        BindingContext = new RolePageTeacherViewModel();
    }
}