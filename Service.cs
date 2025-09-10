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

public class Service
{
    // given by ShellViewModel
    public static ICommand LoggedInCommand;
    public static ICommand LoggedOutCommand;

    public const Int64 UnixMiliseconds24Hours = 24 * 60 * 60 * 1000;

    static UserCredential currentAuthUser = null;

    static UserModel currentUser = null;

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
                AuthTokenAsyncFactory = () => Task.FromResult(auth.User.Credential.IdToken)
            });
    }

    class FirebaseUserModel
    {
        public string? Fullname { get; set; }
        public string? Grade { get; set; }
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

    class FromFirebaseTicket
    {
        public string? SenderId { get; set; }
        public string? Subject { get; set; }
        public Dictionary<string, string>? Helpers { get; set; }
        public Dictionary<string, string>? Topics { get; set; }
        public Dictionary<string, Int64>? OpenTimes { get; set; }
        public bool? IsActive { get; set; }
    }

    // get tickets from everyone
    public static async Task GetAllTicketsFromFB()
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

        if (ticketsFromFB != null)
        {
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

                // TODO: maybe turn subjects into a hashmap for better lookup
                // PARSE SUBJECT
                foreach (Subject sub in subjects)
                {
                    if (sub.Id == tickFromFB.Object.Subject)
                    {
                        parsedTicket.Subject = sub;
                        break;
                    }
                }

                if (tickFromFB.Object.SenderId == auth.User.Uid)
                {
                    // PARSE OPEN TIMES
                    parsedTicket.OpenTimes = new List<long>(tickFromFB.Object.OpenTimes.Values);
                    //.Select(str =>
                    //{
                    //    if (DateTime.TryParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                    //    {
                    //        return result;
                    //    }
                    //    else
                    //    {
                    //        throw new ArgumentException($"Invalid date format: {str}");
                    //    }
                    //})
                    //.ToList();

                    Int64 lastOpenTime = parsedTicket.OpenTimes.Last();

                    Int64 firebaseTime = await GetFirebaseTime();

                    Int64 timeSinceLastOpen = firebaseTime - lastOpenTime;

                    Int64 remainingActiveTime = UnixMiliseconds24Hours - timeSinceLastOpen;

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
                else
                {
                    // PARSE SENDER (only need it's fullname and grade)
                    // the ticket only saves the ID of the sender so we find it in Users
                    var fbSender = await client.Child("Users").Child(tickFromFB.Object.SenderId).OnceSingleAsync<FirebaseUserModel>();

                    if (fbSender != null)
                    {
                        parsedTicket.Sender = new UserModel() { Fullname = fbSender.Fullname };
                        foreach (Grade g in grades)
                        {
                            if (g.Id == fbSender.Grade)
                            {
                                parsedTicket.Sender.Grade = g;
                                break;
                            }
                        }
                    }

                    fbOthersTickets.Add(parsedTicket);
                }
            }

            selfTickets = fbSelfTickets;
            othersTickets = fbOthersTickets;
        }
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

    // get tickets that the user made
    //public static async Task GetRequestedTicketsFromFB()
    //{
    //    List<Ticket> reqFbTickets = new List<Ticket>();

    //    var requestedTicketsFromFB = await client.Child("Tickets").OrderBy("SenderId").EqualTo(auth.User.Uid).OnceAsync<FromFirebaseTicket>();

    //    if (requestedTicketsFromFB != null)
    //    {
    //        foreach (var reqTickFromFB in requestedTicketsFromFB)
    //        {
    //            Ticket parsedRequestedTicket = new Ticket()
    //            {
    //                // in order to update tickets in firebase i saved the key to it
    //                FirebaseKey = reqTickFromFB.Key,
    //                IsActive = reqTickFromFB.Object.IsActive,
    //                Topics = new List<string>(reqTickFromFB.Object.Topics.Values),
    //            };

    //            // PARSE SUBJECT
    //            foreach (Subject sub in subjects)
    //            {
    //                if (sub.Id == reqTickFromFB.Object.Subject)
    //                {
    //                    parsedRequestedTicket.Subject = sub;
    //                    break;
    //                }
    //            }

    //            // PARSE OPEN TIMES
    //            parsedRequestedTicket.OpenTimes = reqTickFromFB.Object.OpenTimes.Values
    //                .Select(str =>
    //                {
    //                    if (DateTime.TryParseExact(str, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
    //                    {
    //                        return result;
    //                    }
    //                    else
    //                    {
    //                        throw new ArgumentException($"Invalid date format: {str}");
    //                    }
    //                })
    //                .ToList();

    //            DateTime lastOpenTime = parsedRequestedTicket.OpenTimes.Last();

    //            DateTime firebaseTime = await GetFirebaseTime();

    //            TimeSpan timeSinceLastOpen = firebaseTime - lastOpenTime;

    //            TimeSpan remainingActiveTime = TimeSpan.FromHours(24) - timeSinceLastOpen;

    //            if (remainingActiveTime <= TimeSpan.Zero)
    //            {
    //                parsedRequestedTicket.IsActive = false;
    //                remainingActiveTime = TimeSpan.Zero;
    //                await client.Child("Tickets").Child(reqTickFromFB.Key).Child("IsActive").PutAsync<bool>(false);
    //            }

    //            parsedRequestedTicket.ActiveTimeSpan = remainingActiveTime;
    //            if (parsedRequestedTicket.IsActive == true)
    //            {
    //                parsedRequestedTicket.ServerActiveTime = $"{(parsedRequestedTicket.ActiveTimeSpan.Days * 24 + parsedRequestedTicket.ActiveTimeSpan.Hours).ToString("00")}:{parsedRequestedTicket.ActiveTimeSpan.Minutes.ToString("00")}:{parsedRequestedTicket.ActiveTimeSpan.Seconds.ToString("00")}";
    //            }
    //            reqFbTickets.Add(parsedRequestedTicket);
    //        }

    //        if (requestedTickets != reqFbTickets)
    //        {
    //            requestedTickets = reqFbTickets;
    //        }
    //    }
    //}

    public static async Task GetAllStaticFBObjects()
    {
        await GetAllSubjectsFromFB();
        await GetAllGradesFromFB();
    }

    public static void InitRealData()
    {
        InitAuth();
    }

    //public static void InitFakeData()
    //{
    //    InitAuth();

    //    //This is fake data, later on data will come from Firbase
    //    //grades.Add(new Grade() { Name = "מורה", Order = 70, Id = 7 });
    //    grades.Add(new Grade() { Name = "ז'", Order = 10, Id = "1" });
    //    grades.Add(new Grade() { Name = "ח'", Order = 20, Id = "2" });
    //    grades.Add(new Grade() { Name = "ט'", Order = 30, Id = "3" });
    //    grades.Add(new Grade() { Name = "י'", Order = 40, Id = "4" });
    //    grades.Add(new Grade() { Name = "י\"א", Order = 50, Id = "5" });
    //    grades.Add(new Grade() { Name = "י\"ב", Order = 60, Id = "6" });

    //    grades.Sort((a, b) => (int)a.Order - (int)b.Order);

    //    subjects.Add(new Subject() { Name = "Math", Order = 10, Id = "1" });
    //    subjects.Add(new Subject() { Name = "English", Order = 20, Id = "2" });
    //    subjects.Add(new Subject() { Name = "Hebrew", Order = 30, Id = "3" });
    //    subjects.Add(new Subject() { Name = "Biology", Order = 40, Id = "4" });
    //    subjects.Add(new Subject() { Name = "Physics", Order = 50, Id = "5" });
    //    subjects.Add(new Subject() { Name = "History", Order = 60, Id = "6" });
    //    subjects.Add(new Subject() { Name = "Bible", Order = 70, Id = "7" });

    //    schools.Add(new School() { Name = "Tchernichovsky", Id = 0, City = "Netanya" });
    //    schools.Add(new School() { Name = "Ort Gutman", Id = 1, City = "Netanya" });
    //    schools.Add(new School() { Name = "Rigler", Id = 2, City = "Netanya" });

    //    msgs.Add(new Message() { Title = "Welcome", Content = "blah blah", Sender = "System", Date = DateTime.Now, MessageType = MessageType.System });
    //    msgs.Add(new Message() { Title = "Hey", Content = "im eldan and this is a message", Sender = "Eldan", Date = DateTime.Now, MessageType = MessageType.Teacher });

    //    DateTime yearAgo = DateTime.Now.AddYears(-1);
    //    DateTime weekAgo = DateTime.Now.AddDays(-7);
    //    DateTime hourAgo = DateTime.Now.AddHours(-1);
    //    DateTime twothousandandeight = new DateTime(2008, 1, 1);

    //    //Dictionary<Subject, List<Grade>> fav = new Dictionary<Subject, List<Grade>>();
    //    //fav.Add(subjects[0], new List<Grade>() { grades[0], grades[1] });

    //    users.Add(new UserModel() { Fullname = "Eldan", Email = "eldan@gmail", PhoneNumber = "0583", Password = "123", UserType = Role.None, Grade = grades[0], RegistrationDate = yearAgo, HelperFavorites = helperFavorites });
    //    users.Add(new UserModel() { Fullname = "Ido Sweed", Email = "ido@gmail.com", PhoneNumber = "0583", Password = "058ASDf@#", UserType = Role.Pupil, Grade = grades[5], RegistrationDate = DateTime.Now });
    //    users.Add(new UserModel() { Fullname = "Ariel", Email = "ariel@gmail", PhoneNumber = "0583", Password = "123", UserType = Role.Pupil, Grade = grades[5], RegistrationDate = hourAgo });
    //    users.Add(new UserModel() { Fullname = "Polina", Email = "polina@gmail", PhoneNumber = "0583", Password = "123", UserType = Role.Pupil, Grade = grades[5], RegistrationDate = weekAgo });


    //    DateTime ServerCurrent = DateTime.Now; // This Will Be Taken From The Database
    //    tickets.Add(new Ticket()
    //    {
    //        OpenTimes = new List<DateTime>() { DateTime.Now },
    //        Subject = subjects[0],
    //        Topics = new List<string>() { "Multiplication", "Division" },
    //        Sender = users[1],
    //        Helpers = new List<UserModel> { users[0] },
    //        IsActive = true,

    //    });
    //    tickets.Add(new Ticket()
    //    {
    //        OpenTimes = new List<DateTime>() { DateTime.Today },
    //        Subject = subjects[1],
    //        Topics = new List<string>() { "Grammar" },
    //        Sender = users[1],
    //        Helpers = new List<UserModel> { users[0] },
    //        IsActive = true,
    //    });
    //    foreach (Ticket ticket in tickets)
    //    {
    //        if (ticket.IsActive == false) continue;
    //        TimeSpan ActiveTime = TimeSpan.FromHours(24) - ServerCurrent.Subtract(ticket.OpenTimes.LastOrDefault());

    //        ticket.ActiveTimeSpan = ActiveTime;
    //        if (ActiveTime.CompareTo(TimeSpan.Zero) <= 0)
    //        {
    //            ticket.IsActive = false;
    //        }
    //        else
    //        {
    //            ticket.ServerActiveTime = $"{(ActiveTime.Days * 24 + ActiveTime.Hours).ToString("00")}:{ActiveTime.Minutes.ToString("00")}:{ActiveTime.Seconds.ToString("00")}";
    //        }
    //    }

    //    //helperFavorites.Add(subjects[0], new List<Grade> { grades[0], grades[1] });
    //    //helperFavorites.Add(subjects[1], new List<Grade> { grades[2] });
    //    Favorite f1 = new Favorite() { Subject = subjects[0], Grades = new List<Grade>() { grades[0], grades[1] } };
    //    Favorite f2 = new Favorite() { Subject = subjects[1], Grades = new List<Grade>() { grades[2] } };
    //    helperFavorites.Add(f1);
    //    helperFavorites.Add(f2);
    //}

    //public static bool AddTicket(Ticket ticket)
    //{
    //    selfTickets.Add(ticket);
    //    return true;
    //}

    // TODO: turn these into firebase functions
    public static async Task<bool> RemoveFavorite(Favorite favorite)
    {
        helperFavorites.Remove(favorite);
        return true;
    }
    public static async Task<bool> EditFavorite(Favorite oldFavorite, Favorite newFavorite)
    {
        foreach (Favorite f in helperFavorites)
        {
            if (f == oldFavorite)
            {
                oldFavorite.Subject = newFavorite.Subject;
                oldFavorite.Grades = newFavorite.Grades;
                break;
            }
        }
        return true;
    }
    public static async Task<bool> AddFavorite(Favorite favorite)
    {
        helperFavorites.Add(favorite);
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
    public static async Task<(bool, string)> HandleTicket(Ticket updatedTicket)
    {
        try
        {
            if (updatedTicket.FirebaseKey == null || updatedTicket.FirebaseKey == "")
            {
                // this is a new ticket that was created
                ToFirebaseTicket newFirebaseTicket = new ToFirebaseTicket()
                {
                    SenderId = auth.User.Uid,
                    Subject = updatedTicket.Subject.Id,
                    IsActive = updatedTicket.IsActive,
                };

                Int64 firebaseTime = await GetFirebaseTime();

                var newFBTicket = await client.Child("Tickets").PostAsync<ToFirebaseTicket>(newFirebaseTicket);

                // TODO: i dont think this is how you handle errors properly
                if (updatedTicket.Topics.Count == 0) throw new ArgumentException("updatedTicket.Topics cannot be empty");

                await client.Child("Tickets").Child(newFBTicket.Key).Child("Topics").PostAsync<string>(updatedTicket.Topics.First());
                await client.Child("Tickets").Child(newFBTicket.Key).Child("OpenTimes").PostAsync<Int64>(firebaseTime);

                return (true, newFBTicket.Key);
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
            return (true, null);
        }
        catch (Exception)
        {
            return (false, null);
        }
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

    public static List<Favorite> GetFavorites()
    {
        return helperFavorites;
    }

    public static void SetFavorites(List<Favorite> favorites)
    {
        helperFavorites = favorites;
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

        currentUser = foundUser;
        return true;
    }

    public static async Task<bool> RequestLoginAsync(string email, string password)
    {
        if (email == null || password == null) return false;
        if (email == "" || password == "") return false;

        try
        {
            var authUser = await auth.SignInWithEmailAndPasswordAsync(email, password);
            currentAuthUser = authUser;

            await GetAllStaticFBObjects();

            return true;
        }
        catch (FirebaseAuthException)
        {
            return false;
        }
    }

    public static bool RequestRegister(string fullname, string phonenumber, string email, string password)
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
