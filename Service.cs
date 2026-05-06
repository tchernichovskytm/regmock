using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;
using regmock.Models;
using regmock.ViewModels;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Input;
using UserModel = regmock.Models.User;

public static class Service
{
    public enum ServiceResult
    {
        Ok,
        TODO,
        InvalidParameter
    }

    // given by ShellViewModel
    public static ICommand LoggedInCommand;
    public static ICommand LoggedOutCommand;

    public static event Action LoggedInEvent;
    public static event Action LoggedOutEvent;

    public static async void OnLogIn()
    {
        LoggedInCommand.Execute(null);
        await GetAllUserFBObjects();
        LoggedInEvent?.Invoke();
    }

    public static void OnLogOut()
    {
        LoggedOutCommand.Execute(null);
        ClearAllUserFBObjects();
        LoggedOutEvent?.Invoke();
    }

    public const Int64 UnixMiliseconds24Hours = 24 * 60 * 60 * 1000;

    static UserCredential currentAuthUser = null;

    //static UserModel currentUser = null;

    // the registration is multiple steps, i need to save the user and build it then send it to FB
    static UserModel tempUser = null;

    static List<UserModel> users = new List<UserModel>();

    static List<Grade> grades = new List<Grade>();

    static List<Subject> subjects = new List<Subject>();

    static List<Ticket> selfTickets = new List<Ticket>();
    static List<Ticket> othersTickets = new List<Ticket>();

    static List<School> schools = new List<School>();

    static List<Message> msgs = new List<Message>();

    static List<Favorite> helperFavorites = new List<Favorite>();

    static FirebaseAuthClient auth;
    static FirebaseClient client;

    static public void InitAuth()
    {
        var config = new FirebaseAuthConfig()
        {
            ApiKey = "AIzaSyDtvfgkLT5rlAvvtaDbbvl5-G5rKSvMLWY",
            AuthDomain = "regmock-3fc47.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            },
        };
        auth = new FirebaseAuthClient(config);

        client = new FirebaseClient(
            @"https://regmock-3fc47-default-rtdb.europe-west1.firebasedatabase.app/",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () =>
                {
                    // TODO: the security is so bad
                    if (auth.User is null) return Task.FromResult(string.Empty);
                    return Task.FromResult(auth.User.Credential.IdToken);
                }
            });
    }

    public static async Task GetAllSubjectsFromFB()
    {
        List<Subject> fbSubjects = new List<Subject>();

        var subjectsFromFB = await client.Child("Subjects").OnceAsync<Subject>();

        if (subjectsFromFB != null)
        {
            foreach (var subFromFB in subjectsFromFB)
            {
                Subject parsedSubject = new Subject() { Id = subFromFB.Key, Name = subFromFB.Object.Name, Order = subFromFB.Object.Order };

                fbSubjects.Add(parsedSubject);
            }

            if (subjects != fbSubjects)
            {
                subjects = fbSubjects;
            }
        }
    }

    public static async Task GetAllGradesFromFB()
    {
        List<Grade> fbGrades = new List<Grade>();

        var gradesFromFB = await client.Child("Grades").OnceAsync<Grade>();

        if (gradesFromFB != null)
        {
            foreach (var gFromFB in gradesFromFB)
            {
                Grade parsedGrade = new Grade() { Id = gFromFB.Key, Name = gFromFB.Object.Name, Order = gFromFB.Object.Order };

                fbGrades.Add(parsedGrade);
            }

            if (grades != fbGrades)
            {
                grades = fbGrades;
            }
        }
    }

    public static async Task GetAllSchoolsFromFB()
    {
        List<School> fbSchools = new List<School>();

        var schoolsFromFB = await client.Child("Schools").OnceAsync<School>();

        if (schoolsFromFB != null)
        {
            foreach (var sFromFB in schoolsFromFB)
            {
                School parsedSchool = new School()
                {
                    Id = sFromFB.Key,
                    Name = sFromFB.Object.Name,
                    City = sFromFB.Object.City,
                    SettlementCode = sFromFB.Object.SettlementCode,
                };

                fbSchools.Add(parsedSchool);
            }

            if (schools != fbSchools)
            {
                schools = fbSchools;
            }
        }
    }

    class FromFirebaseUserDisplay
    {
        public string? Fullname { get; set; }
        public string? Grade { get; set; }
    }

    class FromFirebaseTicket
    {
        public string? SenderId { get; set; }
        public string? Subject { get; set; }
        public Dictionary<string, string>? Helpers { get; set; }
        public Dictionary<string, string>? Topics { get; set; }
        public Dictionary<string, Int64>? OpenTimes { get; set; }
        public bool? IsActive { get; set; }
    }

    // TODO: maybe turn subjects into a hashmap for better lookup
    public static Subject FindSubjectFromId(string subjectId)
    {
        if (string.IsNullOrEmpty(subjectId)) return null;
        foreach (Subject subject in subjects)
        {
            if (subject.Id == subjectId)
            {
                return subject;
            }
        }
        return null;
    }

    public static Grade FindGradeFromId(string gradeId)
    {
        if (string.IsNullOrEmpty(gradeId)) return null;
        foreach (Grade grade in grades)
        {
            if (grade.Id == gradeId)
            {
                return grade;
            }
        }
        return null;
    }

    public static Int64 TicketRemainingTime(Int64 openedTime, Int64 currentTime, Int64 expirationTime)
    {
        return openedTime - (currentTime - expirationTime);
    }

    // get tickets from everyone
    public static async Task<ServiceResult> GetAllTicketsFromFB()
    {
        try
        {
            List<Ticket> fbSelfTickets = new List<Ticket>();
            List<Ticket> fbOthersTickets = new List<Ticket>();

            var ticketsFromFB = await client.Child("Tickets").OnceAsync<FromFirebaseTicket>();

            (ServiceResult, Int64) firebaseTimeResult = await GetFirebaseTime();
            ServiceResult result = firebaseTimeResult.Item1;
            Int64 currentFirebaseTime = firebaseTimeResult.Item2;
            if (result != ServiceResult.Ok)
            {
                // TODO: handle the error maybe or maybe not idk
                return result;
            }

            // TODO: not finished
            // (23/4/2026): why is it not finished? i dont remember
            var validTicketsFromFB = ticketsFromFB.Where(
                ticket =>
                    (ticket.Object.SenderId == auth.User.Uid)
                    ||
                    (TicketRemainingTime(ticket.Object.OpenTimes.Values.Last(), currentFirebaseTime, UnixMiliseconds24Hours) >= 0)
            );

            foreach (var tickFromFB in validTicketsFromFB)
            {
                if (tickFromFB.Object.IsActive == false && tickFromFB.Object.SenderId != auth.User.Uid) continue;

                Ticket parsedTicket = new Ticket()
                {
                    // in order to update tickets in firebase i saved the key to it
                    FirebaseKey = tickFromFB.Key,
                    IsActive = tickFromFB.Object.IsActive,
                    Topics = new List<string>(tickFromFB.Object.Topics.Values),
                };

                // PARSE SUBJECT
                parsedTicket.Subject = FindSubjectFromId(tickFromFB.Object.Subject);

                if (tickFromFB.Object.SenderId == auth.User.Uid)
                {
                    // PARSE OPEN TIMES
                    parsedTicket.OpenTimes = new List<Int64>(tickFromFB.Object.OpenTimes.Values);

                    Int64 remainingActiveTime = TicketRemainingTime(parsedTicket.OpenTimes.Last(), currentFirebaseTime, UnixMiliseconds24Hours);
                    if (remainingActiveTime <= 0)
                    {
                        // TODO: this is problematic, firebase is not a good enough server
                        //       it should mark tickets as inactive by itself
                        parsedTicket.IsActive = false;
                        remainingActiveTime = 0;
                        await client.Child("Tickets").Child(tickFromFB.Key).Child("IsActive").PutAsync<bool>(false);
                    }

                    parsedTicket.ActiveTimeSpan = remainingActiveTime;
                    if (parsedTicket.IsActive == true)
                    {
                        parsedTicket.ServerActiveTime = UnixMilisecondsToHHMMSS(parsedTicket.ActiveTimeSpan);
                    }
                    else
                    {
                        parsedTicket.ServerActiveTime = "";
                    }

                    fbSelfTickets.Add(parsedTicket);
                }
                else // tickFromFB.Object.SenderId != auth.User.Uid
                {
                    // PARSE SENDER (only need it's fullname and grade)
                    // the ticket only saves the ID of the sender so we find it in Users
                    var fbSender = await client.Child("Users").Child(tickFromFB.Object.SenderId).OnceSingleAsync<FromFirebaseUserDisplay>();

                    parsedTicket.Sender = new UserModel() { Fullname = fbSender.Fullname };
                    parsedTicket.Sender.Grade = FindGradeFromId(fbSender.Grade);

                    fbOthersTickets.Add(parsedTicket);
                }
            }
            // this is done because the tickets may fail reading halfway in
            // so we only want to save the tickets after they all passed
            selfTickets = fbSelfTickets;
            othersTickets = fbOthersTickets;
        }
        catch (FirebaseException e)
        {
            return ServiceResult.TODO;
        }
        return ServiceResult.Ok;
    }

    class FromFirebaseFavorite
    {
        public string? Subject { get; set; }
        public List<string>? Grades { get; set; }
    }

    public static async Task<ServiceResult> GetHelperFavoritesFromFB()
    {
        try
        {
            List<Favorite> fbFavorites = new List<Favorite>();

            var favoritesFromFB = await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").OnceAsync<FromFirebaseFavorite>();

            foreach (var favFromFB in favoritesFromFB)
            {
                if (string.IsNullOrEmpty(favFromFB.Object.Subject) || favFromFB.Object.Grades == null) continue;

                Favorite parsedFavorite = new Favorite()
                {
                    Grades = new List<Grade>(),
                    FirebaseKey = favFromFB.Key,
                };
                parsedFavorite.Subject = FindSubjectFromId(favFromFB.Object.Subject);

                foreach (string gradeId in favFromFB.Object.Grades)
                {
                    parsedFavorite.Grades.Add(FindGradeFromId(gradeId));
                }
                fbFavorites.Add(parsedFavorite);
            }
            helperFavorites = fbFavorites;
        }
        catch (FirebaseException ex)
        {
            return ServiceResult.TODO;
        }

        return ServiceResult.Ok;
    }

    public static string UnixMilisecondsToHHMMSS(Int64 unixMilis)
    {
        unixMilis /= 1000;
        Int64 seconds = unixMilis % 60;
        unixMilis /= 60;
        Int64 minutes = unixMilis % 60;
        unixMilis /= 60;
        Int64 hours = unixMilis % 24;
        unixMilis /= 24;
        Int64 days = unixMilis > 0 ? 1 : 0;
        return $"{(days * 24 + hours):00}:{minutes:00}:{seconds:00}";
    }

    public static async Task GetAllStaticFBObjects()
    {
        await GetAllSubjectsFromFB();
        await GetAllGradesFromFB();
        await GetAllSchoolsFromFB();
    }

    public static async Task GetAllUserFBObjects()
    {
        await GetAllTicketsFromFB();
        await GetHelperFavoritesFromFB();
    }

    public static void ClearAllUserFBObjects()
    {
        selfTickets.Clear();
        othersTickets.Clear();
        helperFavorites.Clear();
    }

    // TODO: turn these into firebase functions
    class ToFirebaseFavorite
    {
        public string? Subject { get; set; }
        public List<string>? Grades { get; set; }
    }

    private static ToFirebaseFavorite FavoriteToFirebaseObject(Favorite favorite)
    {
        ToFirebaseFavorite toFirebaseFavorite = new ToFirebaseFavorite
        {
            Subject = favorite.Subject.Id,
            Grades = new List<string>(),
        };
        foreach (Grade g in favorite.Grades)
        {
            toFirebaseFavorite.Grades.Add(g.Id);
        }
        return toFirebaseFavorite;
    }

    public static async Task<ServiceResult> RemoveFavorite(Favorite favorite)
    {
        try
        {
            if (string.IsNullOrEmpty(favorite.FirebaseKey)) return ServiceResult.InvalidParameter;
            await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").Child(favorite.FirebaseKey).DeleteAsync();
        }
        catch (FirebaseException e)
        {
            return ServiceResult.TODO;
        }
        return ServiceResult.Ok;
    }

    public static async Task<ServiceResult> EditFavorite(Favorite oldFavorite, Favorite newFavorite)
    {
        try
        {
            ToFirebaseFavorite toFirebaseFavorite = FavoriteToFirebaseObject(newFavorite);
            await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").Child(oldFavorite.FirebaseKey).PatchAsync<ToFirebaseFavorite>(toFirebaseFavorite);
        }
        catch (FirebaseException e)
        {
            return ServiceResult.TODO;
        }
        return ServiceResult.Ok;
    }

    public static async Task<ServiceResult> AddFavorite(Favorite favorite)
    {
        try
        {
            ToFirebaseFavorite toFirebaseFavorite = FavoriteToFirebaseObject(favorite);
            var newFBFavorite = await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").PostAsync<ToFirebaseFavorite>(toFirebaseFavorite);
            favorite.FirebaseKey = newFBFavorite.Key;
        }
        catch (FirebaseException e)
        {
            return ServiceResult.TODO;
        }
        return ServiceResult.Ok;
    }

    class ToFirebaseTicket
    {
        public string? SenderId { get; set; }
        public string? Subject { get; set; }
        public List<string>? Helpers { get; set; }
        public List<string>? Topics { get; set; }
        public List<string>? OpenTimes { get; set; }
        public bool? IsActive { get; set; }
    }

    // For toggling tickets
    public static async Task<ServiceResult> HandleTicket(Ticket updatedTicket)
    {
        if (string.IsNullOrEmpty(updatedTicket.FirebaseKey))
        {
            // this is a new ticket that was created
            ToFirebaseTicket toFirebaseTicket = new ToFirebaseTicket()
            {
                SenderId = auth.User.Uid,
                Subject = updatedTicket.Subject.Id,
                IsActive = updatedTicket.IsActive,
            };

            (ServiceResult, Int64) firebaseTimeResult = await GetFirebaseTime();
            ServiceResult result = firebaseTimeResult.Item1;
            Int64 firebaseTime = firebaseTimeResult.Item2;
            if (result != ServiceResult.Ok)
            {
                // TODO: handle the error maybe or maybe not idk
                return result;
            }

            var newFBTicket = await client.Child("Tickets").PostAsync<ToFirebaseTicket>(toFirebaseTicket);

            // TODO: i dont think this is how you handle errors properly
            if (updatedTicket.Topics.Count == 0) throw new ArgumentException("updatedTicket.Topics cannot be empty");

            await client.Child("Tickets").Child(newFBTicket.Key).Child("Topics").PostAsync<string>(updatedTicket.Topics.First());
            await client.Child("Tickets").Child(newFBTicket.Key).Child("OpenTimes").PostAsync<Int64>(firebaseTime);

            updatedTicket.FirebaseKey = newFBTicket.Key;
        }
        else if (updatedTicket.FirebaseKey != "")
        {
            // this is a ticket that already exists on firebase
            (ServiceResult, Int64) firebaseTimeResult = await GetFirebaseTime();
            ServiceResult result = firebaseTimeResult.Item1;
            Int64 firebaseTime = firebaseTimeResult.Item2;
            if (result != ServiceResult.Ok)
            {
                // TODO: handle the error maybe or maybe not idk
                return result;
            }

            if (updatedTicket.IsActive != null)
            {
                await client.Child("Tickets").Child(updatedTicket.FirebaseKey).Child("IsActive").PutAsync<bool>(updatedTicket.IsActive.Value);
                if (updatedTicket.IsActive == true)
                {
                    await client.Child("Tickets").Child(updatedTicket.FirebaseKey).Child("OpenTimes").PostAsync<Int64>(firebaseTime);
                    if (updatedTicket.Topics != null)
                    {
                        await client.Child("Tickets").Child(updatedTicket.FirebaseKey).Child("Topics").PostAsync<string>(updatedTicket.Topics.Last());
                    }
                }
            }
        }
        return ServiceResult.Ok;
    }
    public static async Task<ServiceResult> DeleteTicket(Ticket updatedTicket)
    {
        try
        {
            if (!string.IsNullOrEmpty(updatedTicket.FirebaseKey))
            {
                await client.Child("Tickets").Child(updatedTicket.FirebaseKey).DeleteAsync();
            }
            selfTickets.Remove(updatedTicket);
        }
        catch (FirebaseException e)
        {
            return ServiceResult.TODO;
        }
        return ServiceResult.Ok;
    }

    private static async Task<(ServiceResult, Int64)> GetFirebaseTime()
    {
        Int64 ms = 0;
        try
        {
            await client.Child("ServerTime").PutAsync(new Dictionary<string, object> { { ".sv", "timestamp" } });
            ms = await client.Child("ServerTime").OnceSingleAsync<Int64>();
        }
        catch (FirebaseException e)
        {
            return (ServiceResult.TODO, ms);
        }
        //var firebaseTime = DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime;
        return (ServiceResult.Ok, ms);
    }

    public static List<Ticket> GetSelfTickets()
    {
        return selfTickets;
    }

    public static List<Ticket> GetOthersTickets()
    {
        return othersTickets;
    }

    public static List<Ticket> GetAllTickets()
    {
        return [.. selfTickets, .. othersTickets];
    }

    public static List<Subject> GetSubjects()
    {
        return subjects;
    }

    public static List<Grade> GetGrades()
    {
        return grades;
    }

    public static List<School> GetSchools()
    {
        return schools;
    }

    public static List<Favorite> GetHelperFavorites()
    {
        return helperFavorites;
    }

    public static bool RequestFakeLogin(string email, string password)
    {
        UserModel foundUser = null;
        foreach (UserModel p in users)
        {
            if (p.Email == email)
            {
                foundUser = p; break;
            }
        }
        // User not found
        if (foundUser == null) return false;

        // Password doesnt match
        if (foundUser.Password != password)
        {
            return false;
        }

        //currentUser = foundUser;
        return true;
    }

    public static async Task<(ServiceResult, string)> RequestLoginAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email)) return (ServiceResult.InvalidParameter, "Empty Email");
        if (string.IsNullOrEmpty(password)) return (ServiceResult.InvalidParameter, "Empty Password");
        try
        {
            var authUser = await auth.SignInWithEmailAndPasswordAsync(email, password);
            currentAuthUser = authUser;

            OnLogIn();
        }
        catch (FirebaseAuthException e)
        {
            return (ServiceResult.TODO, e.Reason.ToString());
        }
        return (ServiceResult.Ok, null);
    }

    public static bool RequestLogout()
    {
        try
        {
            auth.SignOut();
            OnLogOut();
            selfTickets.Clear();
            othersTickets.Clear();
            helperFavorites.Clear();
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public static void StudentRegister(School school, Grade grade)
    {
        tempUser = new UserModel()
        {
            Role = Role.Student,
            School = school,
            Grade = grade,
        };
    }

    public static void TeacherRegister(School school)
    {
        tempUser = new UserModel()
        {
            Role = Role.UnverifiedTeacher,
            School = school,
            // TODO: add a grade for teachers
        };
    }

    public static void PrincipalRegister(School school)
    {
        tempUser = new UserModel()
        {
            Role = Role.UnverifiedPrincipal,
            // TODO: add a grade for principals
        };
    }

    class ToFirebaseUser
    {
        public string? Fullname { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public Role? Role { get; set; }
        public string? School { get; set; }
        public string? Grade { get; set; }
        public Int64? RegistrationDate { get; set; }
    }

    public static async Task<(ServiceResult, string)> FinalRegisterAsync(string fullname, string phonenumber, string email, string password)
    {
        tempUser.Fullname = fullname;
        tempUser.PhoneNumber = phonenumber;
        tempUser.Email = email;
        tempUser.Password = password;

        try
        {
            var registerAuthUser = await auth.CreateUserWithEmailAndPasswordAsync(tempUser.Email, tempUser.Password, tempUser.Fullname);
            var loginAuthUser = await auth.SignInWithEmailAndPasswordAsync(tempUser.Email, tempUser.Password);
            OnLogIn();
            currentAuthUser = loginAuthUser;

            (ServiceResult, Int64) firebaseTimeResult = await GetFirebaseTime();
            ServiceResult result = firebaseTimeResult.Item1;
            Int64 firebaseTime = firebaseTimeResult.Item2;
            if (result != ServiceResult.Ok)
            {
                // TODO: handle the error maybe or maybe not idk
                return (result, null);
            }

            ToFirebaseUser toFirebaseUser = new ToFirebaseUser()
            {
                Fullname = tempUser.Fullname,
                Password = tempUser.Password,
                PhoneNumber = tempUser.PhoneNumber,
                Email = tempUser.Email,
                Role = tempUser.Role,
                School = tempUser.School.Id,
                Grade = tempUser.Grade.Id,
                RegistrationDate = firebaseTime,
            };

            await client.Child("Users").Child(currentAuthUser.User.Uid).PutAsync<ToFirebaseUser>(toFirebaseUser);
        }
        catch (FirebaseAuthException e)
        {
            return (ServiceResult.TODO, e.Reason.ToString());
        }
        return (ServiceResult.Ok, null);
    }

    public static bool RequestFakeRegister(string fullname, string phonenumber, string email, string password)
    {
        foreach (UserModel p in users)
        {
            if (p.Email == email)
            {
                return false;
            }
        }

        // a generic user from the first step of registration
        users.Add(new UserModel() { Fullname = fullname, PhoneNumber = phonenumber, Email = email, Password = password });

        return true;
    }

    public const int PasswordMinimumLength = 6;

    public enum PasswordResult
    {
        Valid,
        Empty,
        TooShort,
        NoUppercase,
        NoLowercase,
        NoDigits,
    }

    public static PasswordResult CheckValidPassword(string password)
    {
        bool validPassword = false;
        if (string.IsNullOrEmpty(password)) return PasswordResult.Empty;
        if (password.Length == 0) return PasswordResult.Empty;
        if (password.Length < PasswordMinimumLength) return PasswordResult.TooShort;

        //Regex validateGuidRegexCapital = new Regex("^(?=.*?[A-Z]).{1,}$");
        //bool validRegexCapital = validateGuidRegexCapital.IsMatch(password);
        // if (!validRegexCapital) return PasswordResult.NoUppercase;

        //Regex validateGuidRegexLower = new Regex("^(?=.*?[a-z]).{1,}$");
        //bool validRegexLower = validateGuidRegexLower.IsMatch(password);
        // if (!validRegexCapital) return PasswordResult.NoLowercase;

        //Regex validateGuidRegexDigits = new Regex("^(?=.*?[0-9]).{1,}$");
        //bool validRegexDigits = validateGuidRegexDigits.IsMatch(password);
        // if (!validRegexCapital) return PasswordResult.NoDigits;

        return PasswordResult.Valid;
    }
}
