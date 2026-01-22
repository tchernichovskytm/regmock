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
        public async Task InitializeFavoritesAsync()
        {
            // first get the favorites from fb into a list
            await Service.GetHelperFavoritesFromFB();

            // then get it from the saved list
            HelperFavorites = new ObservableCollection<Favorite>(Service.GetHelperFavorites());
            OnPropertyChanged(nameof(HelperFavorites));
        }

        public PickFavoritesPageViewModel()
        {
            InitializeFavoritesAsync();

            AddFavoriteCmd = new Command(async () =>
            {
                await AddFavoriteClick();
            });
            EditFavoriteCmd = new Command(async (object favorite) =>
            {
                if (favorite is Favorite)
                {
                    await EditFavoriteClick((Favorite)favorite);
                }
            });
        }
        #endregion

        #region Functions
        private async Task AddFavoriteClick()
        {
            ICommand favoriteCmd = new Command(async (obj) =>
            {
                if (obj is Favorite)
                {
                    Favorite newFavorite = (Favorite)obj;
                    var success = await Service.AddFavorite(newFavorite);
                    if (success)
                    {
                        HelperFavorites.Add(newFavorite);
                    }
                    else
                    {
                        // TODO: handle error
                    }
                    //Service.SetFavorites(new List<Favorite>(HelperFavorites));
                }
            });
            List<Subject> ExistingSubjects = new List<Subject>();
            foreach (Favorite f in helperFavorites)
            {
                ExistingSubjects.Add(f.Subject);
            }
            await Shell.Current.Navigation.PushModalAsync(new NewFavoritePage(ExistingSubjects, (Command)favoriteCmd), true);
        }
        private async Task EditFavoriteClick(Favorite favorite)
        {
            ICommand modifyFavoriteCmd = new Command(async (object obj) =>
            {
                if (obj is not List<Favorite> || ((List<Favorite>)obj).Count != 2)
                {
                    // invalid object sent back
                    return;
                }
                List<Favorite> modifyFavList = (List<Favorite>)obj;
                Favorite oldFavorite = modifyFavList[0];
                Favorite newFavorite = modifyFavList[1];

                foreach (Favorite fav in HelperFavorites)
                {
                    if (fav.Equals(oldFavorite))
                    {
                        if (newFavorite == null)
                        {
                            var success = await Service.RemoveFavorite(fav);
                            if (success)
                            {
                                HelperFavorites.Remove(fav);
                            }
                            else
                            {
                                // TODO: handle error
                            }
                            return;
                        }
                        else
                        {
                            var success = await Service.EditFavorite(fav, newFavorite);
                            if (success)
                            {
                                fav.Subject = newFavorite.Subject;
                                fav.Grades = newFavorite.Grades;
                            }
                            else
                            {
                                // TODO: handle error
                            }
                            return;
                        }
                    }
                }
            });
            await Shell.Current.Navigation.PushModalAsync(new EditFavoritePage(favorite, (Command)modifyFavoriteCmd), true);
        }
        #endregion
    }
}
