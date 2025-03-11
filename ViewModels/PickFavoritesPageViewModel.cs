using regmock.Models;
namespace regmock.ViewModels
{
    public class PickFavoritesPageViewModel : ViewModelBase
    {
        #region Properties

        private List<Favorite> helperFavorites;

        public List<Favorite> HelperFavorites
        {
            get { return helperFavorites; }
            set
            {
                helperFavorites = value;
                OnPropertyChanged(nameof(helperFavorites));
            }
        }


        #endregion

        #region Commands
        #endregion

        #region Constructor
        public PickFavoritesPageViewModel()
        {
            HelperFavorites = Service.GetFavorites();
        }
        #endregion

        #region Functions
        #endregion
    }
}
