using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Utils;

namespace CEC
{
    public partial class MainWindow : Window
    {

        static ApplicationContext db = new ApplicationContext();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();

            //SeedTheDb();

            Update();
        }


        


        private void AddClick(object sender, RoutedEventArgs e)
        {
            PersonsWindow PersonsWindow = new PersonsWindow(new Person());
            if (PersonsWindow.ShowDialog() == true)
            {
                Person Person = PersonsWindow.Person;
                db.Persons.Add(Person);
                db.SaveChanges();
            }
            Update();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            PersonForDC? person = personsList.SelectedItem as PersonForDC;
            if (person is null) return;
            Person personToRemove = db.Persons.FirstOrDefault(x => x.Id == person.PersonId);

            db.Persons.Remove(personToRemove);
            db.SaveChanges();
            Update();
        }

        private void VotesClick(object sender, RoutedEventArgs e)
        {
            VotesWindow VotesWindow = new VotesWindow();
            if (VotesWindow.ShowDialog() == true)
            {

            }
        }

        private void ResultsClick(object sender, RoutedEventArgs e)
        {
            ResultsWindow resultsWindow = new ResultsWindow();
            if (resultsWindow.ShowDialog() == true)
            {

            }
        }

        private void UpdateClick(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void Update()
        {
            var PersonQuery = from p in db.Persons
                              join r in db.Roles on p.RoleId equals r.Id
                              join perm in db.PermissionsToVote on p.PermissionToVoteId equals perm.Id
                              select new PersonForDC(p.Name, p.Surname, r.Name, perm.Permission, p.Id);

            DataContext = ToObservableCollection<PersonForDC>(PersonQuery);
        }

        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }


        public void SeedTheDb()
        {
            PermissionToVote permissionToVote1 = new PermissionToVote { Permission = "Не може голосувати" };
            PermissionToVote permissionToVote2 = new PermissionToVote { Permission = "Може голосувати" };

            Role role1 = new Role { Name = "Виборець" };
            Role role2 = new Role { Name = "Кандидат" };

            Person person1 = new Person { Name = "Іван", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person2 = new Person { Name = "Василь", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person3 = new Person { Name = "Петро", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person4 = new Person { Name = "Степан", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person5 = new Person { Name = "Леонід", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person6 = new Person { Name = "Ім'я", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person7 = new Person { Name = "ОлександЕр", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person8 = new Person { Name = "Вадим", Surname = "Виборенко", RoleId = 1, PermissionToVoteId = 2 };
            Person person9 = new Person { Name = "Андрій", Surname = "НЕможенко", RoleId = 1, PermissionToVoteId = 1 };

            Person cand1 = new Person { Name = "Іван", Surname = "Кандидатенко", RoleId = 2, PermissionToVoteId = 2 };
            Person cand2 = new Person { Name = "Василь", Surname = "Кандидатенко", RoleId = 2, PermissionToVoteId = 2 };
            Person cand3 = new Person { Name = "Петро", Surname = "КандеНЕтенко", RoleId = 2, PermissionToVoteId = 1 };

            Vote vote1 = new Vote { ElectorId = 1, CandidateId = 10 };
            Vote vote2 = new Vote { ElectorId = 7, CandidateId = 10 };
            Vote vote3 = new Vote { ElectorId = 8, CandidateId = 12 };

            db.PermissionsToVote.AddRange(permissionToVote1, permissionToVote2);
            db.Roles.AddRange(role1, role2);
            db.SaveChanges();

            db.Persons.AddRange(person1, person2, person3, person4, person5, person6, person7, person8, person9);
            db.Persons.AddRange(cand1, cand2, cand3);

            db.SaveChanges();

            db.Votes.AddRange(vote1, vote2, vote3);
            db.SaveChanges();
        }


        public static bool SendBulletin(byte[] encOriginalData, byte[] encSignedData, RSAParameters publicKey)
        {
            byte[] originalData = XOR.DoXOROperation(encOriginalData, XORKey);
            byte[] signedData = XOR.DoXOROperation(encSignedData, XORKey);


            if (!VerifySignedHash(originalData, signedData, publicKey))
            {
                ShowMessage("Хеш ЕЦП не співпав з хешом бюлетеня");
                return false;
            }

            string strOriginalData = Encoding.UTF8.GetString(originalData);
            strOriginalData = strOriginalData.Trim(' ');

            string[] words = strOriginalData.Split('|');

            int ElectorId = Convert.ToInt32(words[0]);
            int CandidateId = Convert.ToInt32(words[1]);

            Person Elector = db.Persons.FirstOrDefault(p => p.Id == ElectorId);
            Person Candidate = db.Persons.FirstOrDefault(p => p.Id == CandidateId);

            if (Elector is null || Candidate is null)
            {
                ShowMessage("Виборця або кандидата не знайдено");
                return false;
            }


            if (Elector.PermissionToVoteId == 1)
            {
                ShowMessage("Ви не можете голосувати");
                return false;
            }

            var votesQuery = from v in db.Votes
                             where v.ElectorId == ElectorId
                             select v;

            var votesList = votesQuery.ToList();
            if (votesList.Count != 0)
            {
                ShowMessage("Ви вже голосували");
                return false;
            }

            Vote vote = new Vote();
            vote.ElectorId = ElectorId;
            vote.CandidateId = CandidateId;

            db.Votes.Add(vote);
            db.SaveChanges();

            ShowMessage("Ваш голос враховано");

            return true;
        }

        public static byte[] XORKey = GetXORKey(128);

        public static byte[] GetXORKey(int len)
        {
            Byte[] bKey = new Byte[len];

            Random r = new Random();
            r.NextBytes(bKey);

            return bKey;
        }

        public static bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, RSAParameters Key)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                return RSAalg.VerifyData(DataToVerify, SHA256.Create(), SignedData);
            }
            catch (CryptographicException e)
            {
                return false;
            }
        }
        public static void ShowMessage(string message)
        {
            string messageBoxText = message;
            string caption = ":-}";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

        }

        class PersonForDC
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Role { get; set; }
            public string Permission { get; set; }
            public int PersonId { get; set; }
            public PersonForDC(string name, string surname, string role, string permission, int personId)
            {
                Name = name;
                Surname = surname;
                Role = role;
                Permission = permission;
                PersonId = personId;
            }
        }
    }
}
