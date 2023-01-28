using Model;
using System.Windows;
using System.Windows.Controls;

namespace CEC
{
    public partial class PersonsWindow : Window
    {
        public Person Person { get; private set; }

        public PersonsWindow(Person person)
        {
            Person = person;
            InitializeComponent();
        }

        void AddClick(object sender, RoutedEventArgs e)
        {

            Person.Name = TextBoxName.Text;
            Person.Surname = TextBoxSurname.Text;

            ComboBoxItem selectedRole = (ComboBoxItem)Role.SelectedItem;
            ComboBoxItem selectedPermission = (ComboBoxItem)Permission.SelectedItem;

            if (selectedRole.Content.ToString() == "Виборець")
            {
                Person.RoleId = 1;
            }
            else
            {
                Person.RoleId = 2;
            }

            if (selectedPermission.Content.ToString() == "Не може голосувати")
            {
                Person.PermissionToVoteId = 1;
            }
            else
            {
                Person.PermissionToVoteId = 2;
            }


            DialogResult = true;
        }
    }
}
