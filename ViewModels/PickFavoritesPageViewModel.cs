using Microsoft.Maui.Animations;
using regmock.Models;
using System.Windows.Input;

using regmock.Views;
using System.Collections.ObjectModel;

namespace regmock.ViewModels
{
    public class PickFavoritesPageViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<Favorite> helperFavorites;

        public ObservableCollection<Favorite> HelperFavorites
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
        public ICommand AddFavoriteCmd { get; set; }
        public ICommand EditFavoriteCmd { get; set; }
        #endregion

        #region Constructor
        public PickFavoritesPageViewModel()
        {
            HelperFavorites = new ObservableCollection<Favorite>(Service.GetFavorites());

            AddFavoriteCmd = new Command(() =>
            {
                AddFavoriteClick();
                Service.SetFavorites(new List<Favorite>(HelperFavorites));
            });
            EditFavoriteCmd = new Command((object favorite) =>
            {
                if (favorite is Favorite)
                {
                    EditFavoriteClick((Favorite)favorite);
                    Service.SetFavorites(new List<Favorite>(HelperFavorites));
                }
            });
        }
        #endregion

        #region Functions
        private async void AddFavoriteClick()
        {
            ICommand favoriteCmd = new Command((newFavorite) =>
            {
                if (newFavorite is Favorite)
                {
                    HelperFavorites.Add((Favorite)newFavorite);
                }
            });
            List<Subject> ExistingSubjects = new List<Subject>();
            foreach (Favorite f in helperFavorites) {
                ExistingSubjects.Add(f.Subject);
            }
            await Shell.Current.Navigation.PushModalAsync(new NewPreferencePage(ExistingSubjects, (Command)favoriteCmd), true);
        }
        private async void EditFavoriteClick(Favorite favorite)
        {
            ICommand modifyFavoriteCmd = new Command((modifyFavList) =>
            {
                if (modifyFavList is List<Favorite> && ((List<Favorite>)modifyFavList).Count == 2)
                {
                    Favorite oldFavorite = ((List<Favorite>)modifyFavList)[0];
                    Favorite newFavorite = ((List<Favorite>)modifyFavList)[1];

                    foreach (Favorite fav in HelperFavorites)
                    {
                        if (fav.Equals(oldFavorite))
                        {
                            if (newFavorite == null)
                            {
                                HelperFavorites.Remove(fav);
                                return;
                            }
                            else
                            {
                                fav.Subject = newFavorite.Subject;
                                fav.Grades = newFavorite.Grades;
                                return;
                            }
                        }
                    }
                }
            });
            await Shell.Current.Navigation.PushModalAsync(new EditPreferencePage(favorite, (Command)modifyFavoriteCmd), true);
        }
        #endregion
    }
}
