using regmock.Models;
using System;
using System.Text.Json;

using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Database.Query;

using UserModel = regmock.Models.User;
using regmock.ViewModels;
using System.ComponentModel;
using System.Linq;
using System.Globalization;
using System.Windows.Input;

public static class Service
{
    // given by ShellViewModel
    public static ICommand LoggedInCommand;
    public static ICommand LoggedOutCommand;

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
    public static async Task<bool> GetAllTicketsFromFB()
    {
        List<Ticket> fbSelfTickets = new List<Ticket>();
        List<Ticket> fbOthersTickets = new List<Ticket>();

        // TODO: we need to switch the date time saved in tickets to unix miliseconds in order to query them easialy,
        //       this requires changing a bunch of code, after that i need to query the tickets like this:
        //                            var ticketsQuery = await client
        //                            .Child("Tickets")
        //                            .OrderBy("lastOpened")              // Order by the lastOpened timestamp (Unix milliseconds)
        //                            .StartAt(twentyFourHoursAgoMillis)  // Only get tickets with lastOpened >= 24 hours ago
        //                            .EndAt(currentMillis)               // Optionally, limit to the current time (you may omit this if not necessary)
        //                            .OnceAsync<FromFirebaseTicket>();

        var ticketsFromFB = await client.Child("Tickets").OnceAsync<FromFirebaseTicket>();

        Int64 currentFirebaseTime = await GetFirebaseTime();

        // TODO: not finished
        //var validTicketsFromFB = ticketsFromFB.Where(ticket => TicketRemainingTime(ticket.Object.OpenTimes.Values.Last(), currentFirebaseTime, UnixMiliseconds24Hours) >= 0);

        if (ticketsFromFB == null) return false;

        foreach (var tickFromFB in ticketsFromFB)
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

                if (fbSender == null) return false;
                parsedTicket.Sender = new UserModel() { Fullname = fbSender.Fullname };
                parsedTicket.Sender.Grade = FindGradeFromId(fbSender.Grade);

                fbOthersTickets.Add(parsedTicket);
            }
            // this is done because the tickets may fail reading halfway in
            // so we only want to save the tickets after they all passed
            selfTickets = fbSelfTickets;
            othersTickets = fbOthersTickets;
        }
        return true;
    }

    class FromFirebaseFavorite
    {
        public string? Subject { get; set; }
        public List<string>? Grades { get; set; }
    }

    public static async Task<bool> GetHelperFavoritesFromFB()
    {
        try
        {
            List<Favorite> fbFavorites = new List<Favorite>();

            var favoritesFromFB = await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").OnceAsync<FromFirebaseFavorite>();

            if (favoritesFromFB == null) return false;
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
        catch (Exception ex) {
            return false;
        }

        return true;
    }

    public static string UnixMilisecondsToHHMMSS(Int64 unixMilis)
    {
        //return $"{(ts.Days * 24 + ts.Hours).ToString("00")}:{ts.Minutes.ToString("00")}:{ts.Seconds.ToString("00")}";
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

    public static void GetAllStaticFBObjects()
    {
        GetAllSubjectsFromFB();
        GetAllGradesFromFB();
        GetAllSchoolsFromFB();
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

    public static async Task<bool> RemoveFavorite(Favorite favorite)
    {
        if (string.IsNullOrEmpty(favorite.FirebaseKey)) return false;

        await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").Child(favorite.FirebaseKey).DeleteAsync();

        return true;
    }

    public static async Task<bool> EditFavorite(Favorite oldFavorite, Favorite newFavorite)
    {
        ToFirebaseFavorite toFirebaseFavorite = FavoriteToFirebaseObject(newFavorite);

        await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").Child(oldFavorite.FirebaseKey).PatchAsync<ToFirebaseFavorite>(toFirebaseFavorite);

        return true;
    }

    public static async Task<bool> AddFavorite(Favorite favorite)
    {
        ToFirebaseFavorite toFirebaseFavorite = FavoriteToFirebaseObject(favorite);

        var newFBFavorite = await client.Child("Users").Child(auth.User.Uid).Child("HelperFavorites").PostAsync<ToFirebaseFavorite>(toFirebaseFavorite);
        favorite.FirebaseKey = newFBFavorite.Key;

        return true;
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
    public static async Task<bool> HandleTicket(Ticket updatedTicket)
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

            Int64 firebaseTime = await GetFirebaseTime();

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
            Int64 firebaseTime = await GetFirebaseTime();

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
        return true;
    }
    public static async Task<bool> DeleteTicket(Ticket updatedTicket)
    {
        if (!string.IsNullOrEmpty(updatedTicket.FirebaseKey))
        {
            await client.Child("Tickets").Child(updatedTicket.FirebaseKey).DeleteAsync();
        }
        return true;
    }

    private static async Task<Int64> GetFirebaseTime()
    {
        await client.Child("ServerTime").PutAsync(new Dictionary<string, object> { { ".sv", "timestamp" } });
        var millis = await client.Child("ServerTime").OnceSingleAsync<Int64>();
        //var firebaseTime = DateTimeOffset.FromUnixTimeMilliseconds(millis).UtcDateTime;
        return millis;
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

    // TODO: give back useful error message | 4/1/26: this is likely not possible
    public static async Task<bool> RequestLoginAsync(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return false;

            var authUser = await auth.SignInWithEmailAndPasswordAsync(email, password);
            currentAuthUser = authUser;

            LoggedInCommand.Execute(null);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public static bool RequestLogoutAsync()
    {
        try
        {
            auth.SignOut();
            LoggedOutCommand.Execute(null);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public static async Task<bool> InitialRegisterAsync(string fullname, string phonenumber, string email, string password)
    {
        // TODO: verify email does not exist already

        tempUser = new UserModel()
        {
            Fullname = fullname,
            PhoneNumber = phonenumber,
            Email = email,
            Password = password,
            Role = Role.None,
            RegistrationDate = await GetFirebaseTime(),
        };
        return true;
    }

    public static async Task<bool> StudentRegisterAsync(School school, Grade grade)
    {
        tempUser.Role = Role.Student;
        tempUser.School = school;
        tempUser.Grade = grade;

        // TODO: verify that the 2 users are the same and there are no errors
        var registerAuthUser = await auth.CreateUserWithEmailAndPasswordAsync(tempUser.Email, tempUser.Password, tempUser.Fullname);
        var loginAuthUser = await auth.SignInWithEmailAndPasswordAsync(tempUser.Email, tempUser.Password);
        LoggedInCommand.Execute(null);
        currentAuthUser = loginAuthUser;
        await client.Child("Users").Child(currentAuthUser.User.Uid).PutAsync<UserModel>(tempUser);
        return true;
    }

    //public static async Task<bool> RequestRegisterAsync(string fullname, string phonenumber, string email, string password)
    //{
    //    if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(phonenumber) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return false;

    //    try
    //    {
    //        var authUser = await auth.CreateUserWithEmailAndPasswordAsync(email, password, fullname);
    //        UserModel newUser = new UserModel()
    //        {
    //            Fullname = fullname,
    //            PhoneNumber = phonenumber,
    //            Email = email,
    //            Password = password,
    //            UserType = Role.None,
    //            RegistrationDate = await GetFirebaseTime(),
    //        };
    //        tempUser = newUser;

    //        //client.Child("Users").Child(authUser.User.Uid).PostAsync<UserModel>(authUser);

    //        return true;
    //    }
    //    catch (FirebaseAuthException)
    //    {
    //        return false;
    //    }
    //}

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
}
