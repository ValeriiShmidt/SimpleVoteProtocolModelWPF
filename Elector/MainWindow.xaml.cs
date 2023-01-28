using Model;
using Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Elector
{
    public partial class MainWindow : Window
    {

        public JoinedVoter Voter { get; private set; }
        ApplicationContext db = new ApplicationContext();


        public MainWindow()
        {
            db.Database.EnsureCreated();

            InitializeComponent();
        }

        public void ConnectClick(object sender, RoutedEventArgs e)
        {
            string inputName = TextBoxName.Text;
            string inputSurname = TextBoxSurname.Text;


            var personsQuery = from p in db.Persons
                               join r in db.Roles on p.RoleId equals r.Id
                               join perm in db.PermissionsToVote on p.PermissionToVoteId equals perm.Id
                               where p.Name == inputName && p.Surname == inputSurname
                               select new JoinedVoter(p, r.Name, perm.Permission);


            Voter = personsQuery.FirstOrDefault();

            if (Voter is null)
            {
                string messageBoxText = "Користувача не знайдено: " +
                    inputName + " " + inputSurname;
                string caption = ":-(";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
                return;
            }

            else
            {
                string messageBoxText = "Користувача знайдено в системі." + "\n"
                                      + "Ваш ID: " + Voter.Person.Id + "\n"
                                      + "Ваша роль: " + Voter.Role + "\n"
                                      + "Ваш дозвіл: " + Voter.PermissionToVote + "\n"
                                      + "\nОберіть кандидата";
                string caption = ":-)";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }

            var candidatesQuery = from p in db.Persons
                                  join r in db.Roles on p.RoleId equals r.Id
                                  where r.Name == "Кандидат"
                                  select new CandidateForDC(p.Name, p.Surname, p.Id);

            DataContext = ToObservableCollection<CandidateForDC>(candidatesQuery);
        }

        public void VoteClick(object sender, RoutedEventArgs e)
        {

            CandidateForDC? candidate = candidatesList.SelectedItem as CandidateForDC;
            if (candidate is null) return;
            Person votedCandidate = db.Persons.FirstOrDefault(x => x.Id == candidate.PersonId);

            string message = Voter.Person.Id.ToString() + "|" + candidate.PersonId.ToString();
            message = message.PadLeft(128, ' ');


            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);

            RSAParameters RSAPrivateKey = RSA.ExportParameters(true);
            RSAParameters RSAPublicKey = RSA.ExportParameters(false);


            byte[] originalData = Encoding.UTF8.GetBytes(message);
            byte[] signedData;

            signedData = HashAndSignBytes(originalData, RSAPrivateKey);

            byte[] encOriginalData = XOR.DoXOROperation(originalData, CEC.MainWindow.XORKey);
            byte[] encSignedData = XOR.DoXOROperation(signedData, CEC.MainWindow.XORKey);


            bool recieved = CEC.MainWindow.SendBulletin(encOriginalData, encSignedData, RSAPublicKey);

        }

        public class JoinedVoter
        {
            public Person Person { get; set; }
            public string Role { get; set; }
            public string PermissionToVote { get; set; }
            public JoinedVoter(Person person, string role, string permissionToVote)
            {
                Person = person;
                Role = role;
                PermissionToVote = permissionToVote;
            }
        }

        class CandidateForDC
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public int PersonId { get; set; }
            public CandidateForDC(string name, string surname, int personId)
            {
                Name = name;
                Surname = surname;
                PersonId = personId;
            }
        }

        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }

        public static byte[] HashAndSignBytes(byte[] DataToSign, RSAParameters Key)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportParameters(Key);

                return RSAalg.SignData(DataToSign, SHA256.Create());
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }
    }

}
