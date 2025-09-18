using regmock.ViewModels;

namespace regmock.Views;

public partial class RolePageStudent : ContentPage
{
    public RolePageStudent()
    {
        InitializeComponent();

        BindingContext = new RolePageStudentViewModel();
    }
}