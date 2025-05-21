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

public class Service
{
    static UserCredential currentAuthUser = null;

    static UserModel currentUser = null;

    static List<UserModel> users = new List<UserModel>();

    static List<Grade> grades = new List<Grade>();

    static List<Subject> subjects = new List<Subject>();

    static List<Ticket> tickets = new List<Ticket>();

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

    class FirebaseTicket
    {
        public string SenderId { get; set; }
        public string Subject { get; set; }
        public string[] Helpers { get; set; }
        public string[] Topics { get; set; }
        public DateTime[] OpenTimes { get; set; }
        public bool isActive { get; set; }
    }

    public static void GetAllSubjectsFromFB()
    {
        List<Subject> fbSubjects = new List<Subject>();

        var subjectsFromFB = client.Child("Subjects").OnceAsync<Subject>().Result;

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

    public static void GetAllTicketsFromFB()
    {
        List<Ticket> fbTickets = new List<Ticket>();

        var ticketsFromFB = client.Child("Tickets").OnceAsync<FirebaseTicket>().Result;

        foreach (var tickFromFB in ticketsFromFB)
        {
            Ticket parsedTicket = new Ticket()
            {
                // in order to update tickets in firebase i saved the key to it
                FirebaseKey = tickFromFB.Key,
                IsActive = tickFromFB.Object.isActive,
                Topics = new List<string>(tickFromFB.Object.Topics),
                OpenTimes = new List<DateTime>(tickFromFB.Object.OpenTimes),
            };

            // PARSE SUBJECT
            foreach (Subject sub in subjects)
            {
                if (sub.Id == tickFromFB.Object.Subject)
                {
                    parsedTicket.Subject = sub;
                    break;
                }
            }

            // PARSE SENDER


            // PARSE HELPERS

            fbTickets.Add(parsedTicket);
        }

        if (tickets != fbTickets)
        {
            tickets = fbTickets;
        }
    }

    public static void GetAllStaticFBObjects()
    {
        GetAllSubjectsFromFB();
    }

    public static void InitRealData()
    {
        InitAuth();
    }

    public static void InitFakeData()
    {
        InitAuth();

        //This is fake data, later on data will come from Firbase
        //grades.Add(new Grade() { Name = "מורה", Order = 70, Id = 7 });
        grades.Add(new Grade() { Name = "ז'", Order = 10, Id = 1 });
        grades.Add(new Grade() { Name = "ח'", Order = 20, Id = 2 });
        grades.Add(new Grade() { Name = "ט'", Order = 30, Id = 3 });
        grades.Add(new Grade() { Name = "י'", Order = 40, Id = 4 });
        grades.Add(new Grade() { Name = "י\"א", Order = 50, Id = 5 });
        grades.Add(new Grade() { Name = "י\"ב", Order = 60, Id = 6 });

        grades.Sort((a, b) => (int)a.Order - (int)b.Order);

        subjects.Add(new Subject() { Name = "Math", Order = 10, Id = "1" });
        subjects.Add(new Subject() { Name = "English", Order = 20, Id = "2" });
        subjects.Add(new Subject() { Name = "Hebrew", Order = 30, Id = "3" });
        subjects.Add(new Subject() { Name = "Biology", Order = 40, Id = "4" });
        subjects.Add(new Subject() { Name = "Physics", Order = 50, Id = "5" });
        subjects.Add(new Subject() { Name = "History", Order = 60, Id = "6" });
        subjects.Add(new Subject() { Name = "Bible", Order = 70, Id = "7" });

        schools.Add(new School() { Name = "Tchernichovsky", Id = 0, City = "Netanya" });
        schools.Add(new School() { Name = "Ort Gutman", Id = 1, City = "Netanya" });
        schools.Add(new School() { Name = "Rigler", Id = 2, City = "Netanya" });

        msgs.Add(new Message() { Title = "Welcome", Content = "blah blah", Sender = "System", Date = DateTime.Now, MessageType = MessageType.System });
        msgs.Add(new Message() { Title = "Hey", Content = "im eldan and this is a message", Sender = "Eldan", Date = DateTime.Now, MessageType = MessageType.Teacher });

        DateTime yearAgo = DateTime.Now.AddYears(-1);
        DateTime weekAgo = DateTime.Now.AddDays(-7);
        DateTime hourAgo = DateTime.Now.AddHours(-1);
        DateTime twothousandandeight = new DateTime(2008, 1, 1);

        //Dictionary<Subject, List<Grade>> fav = new Dictionary<Subject, List<Grade>>();
        //fav.Add(subjects[0], new List<Grade>() { grades[0], grades[1] });

        users.Add(new UserModel() { Fullname = "Eldan", Email = "eldan@gmail", PhoneNumber = "0583", Password = "123", UserType = Role.None, Grade = grades[0], RegistrationDate = yearAgo, HelperFavorites = helperFavorites });
        users.Add(new UserModel() { Fullname = "Ido Sweed", Email = "ido@gmail.com", PhoneNumber = "0583", Password = "058ASDf@#", UserType = Role.Pupil, Grade = grades[5], RegistrationDate = DateTime.Now });
        users.Add(new UserModel() { Fullname = "Ariel", Email = "ariel@gmail", PhoneNumber = "0583", Password = "123", UserType = Role.Pupil, Grade = grades[5], RegistrationDate = hourAgo });
        users.Add(new UserModel() { Fullname = "Polina", Email = "polina@gmail", PhoneNumber = "0583", Password = "123", UserType = Role.Pupil, Grade = grades[5], RegistrationDate = weekAgo });


        DateTime ServerCurrent = DateTime.Now; // This Will Be Taken From The Database
        tickets.Add(new Ticket()
        {
            OpenTimes = new List<DateTime>() { DateTime.Now },
            Subject = subjects[0],
            Topics = new List<string>() { "Multiplication", "Division" },
            Sender = users[1],
            Helpers = new List<UserModel> { users[0] },
            IsActive = true,

        });
        tickets.Add(new Ticket()
        {
            OpenTimes = new List<DateTime>() { DateTime.Today },
            Subject = subjects[1],
            Topics = new List<string>() { "Grammar" },
            Sender = users[1],
            Helpers = new List<UserModel> { users[0] },
            IsActive = true,
        });
        foreach (Ticket ticket in tickets)
        {
            if (ticket.IsActive == false) continue;
            TimeSpan ActiveTime = TimeSpan.FromHours(24) - ServerCurrent.Subtract(ticket.OpenTimes.LastOrDefault());

            ticket.ActiveTimeSpan = ActiveTime;
            if (ActiveTime.CompareTo(TimeSpan.Zero) <= 0)
            {
                ticket.IsActive = false;
            }
            else
            {
                ticket.ServerActiveTime = $"{(ActiveTime.Days * 24 + ActiveTime.Hours).ToString("00")}:{ActiveTime.Minutes.ToString("00")}:{ActiveTime.Seconds.ToString("00")}";
            }
        }

        //helperFavorites.Add(subjects[0], new List<Grade> { grades[0], grades[1] });
        //helperFavorites.Add(subjects[1], new List<Grade> { grades[2] });
        Favorite f1 = new Favorite() { Subject = subjects[0], Grades = new List<Grade>() { grades[0], grades[1] } };
        Favorite f2 = new Favorite() { Subject = subjects[1], Grades = new List<Grade>() { grades[2] } };
        helperFavorites.Add(f1);
        helperFavorites.Add(f2);
    }

    public static bool AddTicket(Ticket ticket)
    {
        tickets.Add(ticket);
        return true;
    }
    public static bool RemoveFavorite(Favorite favorite)
    {
        helperFavorites.Remove(favorite);
        return true;
    }
    public static bool EditFavorite(Favorite oldFavorite, Favorite newFavorite)
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
    public static bool AddFavorite(Favorite favorite)
    {
        helperFavorites.Add(favorite);
        return true;
    }

    // For toggling tickets
    public static bool HandleTicket(Ticket updatedTicket)
    {
        if (updatedTicket.FirebaseKey == null)
        {
            // this is a new ticket that was created
            FirebaseTicket firebaseTicket = new FirebaseTicket() {
                Subject = updatedTicket.Subject.Id,
                isActive = updatedTicket.IsActive,

            };

            client.Child("Tickets").PutAsync<FirebaseTicket>()
        } else if (updatedTicket.FirebaseKey != "")
        {
            // this is a ticket that already exists on firebase
        }
        return true;
    }

    public static List<Ticket> GetTickets()
    {
        return tickets;
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

            GetAllStaticFBObjects();

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
