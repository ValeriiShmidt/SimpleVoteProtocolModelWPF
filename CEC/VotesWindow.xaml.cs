using Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CEC
{
    public partial class VotesWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        public VotesWindow()
        {
            InitializeComponent();
            Loaded += VotesWindowLoaded;
        }

        private void VotesWindowLoaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();

            var VoteQuery = from v in db.Votes
                            join p in db.Persons on v.ElectorId equals p.Id
                            select new VoteForDC(v.Elector.Name + " " + v.Elector.Surname,
                                                 v.Candidate.Name + " " + v.Candidate.Surname);

            DataContext = ToObservableCollection<VoteForDC>(VoteQuery);
        }

        class VoteForDC
        {
            public string Elector { get; set; }
            public string Candidate { get; set; }
            public VoteForDC(string elector, string candidate)
            {
                Elector = elector;
                Candidate = candidate;
            }
        }

        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }
    }
}
