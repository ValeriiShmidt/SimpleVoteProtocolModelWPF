using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CEC
{
    public partial class ResultsWindow : Window
    {
        ApplicationContext db = new ApplicationContext();

        public ResultsWindow()
        {
            InitializeComponent();
            Loaded += resultsWindowLoaded;
        }

        private void resultsWindowLoaded(object sender, RoutedEventArgs e)
        {
            db.Database.EnsureCreated();

            var VotesList = db.Votes.ToList();

            var CandidatesQuery = from p in db.Persons
                                  where p.RoleId == 2
                                  select new ResultsForDC(p.Name + " " + p.Surname, p.Id, 0);

            var CandidatesList = CandidatesQuery.ToList();

            foreach (var vote in VotesList)
            {
                foreach (var candidate in CandidatesList)
                {
                    if (candidate.Id == vote.CandidateId)
                    {
                        candidate.VotesCount++;
                        break;
                    }
                }
            }

            CandidatesList.Sort();

            DataContext = ToObservableCollection<ResultsForDC>(CandidatesList);
        }


        class ResultsForDC : IComparable<ResultsForDC>
        {
            public string Elector { get; set; }
            public int Id { get; set; }
            public int VotesCount { get; set; }
            public ResultsForDC(string elector, int id, int votesCount)
            {
                Elector = elector;
                Id = id;
                VotesCount = votesCount;
            }

            public int CompareTo(ResultsForDC other)
            {
                if (this.VotesCount > other.VotesCount)
                {
                    return -1;
                }
                else if (this.VotesCount < other.VotesCount)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        public ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }
    }
}
