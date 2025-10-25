using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMCS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // load users into selector and navigate to Home page
            try
            {
                using var scope = App.ServiceProvider!.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var users = db.Users.ToList();
                UserSelector.ItemsSource = users;
                var first = users.FirstOrDefault();
                if (first != null)
                {
                    UserSelector.SelectedItem = first;
                    Session.CurrentUser = first;
                }

                // Navigate to home page
                MainFrame.Navigate(new HomePage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }

            UserSelector.SelectionChanged += UserSelector_SelectionChanged;
        }

        private void UserSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserSelector.SelectedItem is User u)
            {
                Session.CurrentUser = u;
            }
        }

        private void NavHome_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage());
        }

        private void NavLecturer_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LecturerPage());
        }

        private void NavCoordinator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CoordinatorPage());
        }

        private void NavManager_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManagerPage());
        }

        private void NavClaimForm_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClaimFormPage());
        }
    }
}
