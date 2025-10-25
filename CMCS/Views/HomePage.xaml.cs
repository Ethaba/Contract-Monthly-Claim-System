using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMCS.Views
{
    public partial class HomePage : Page
    {
        // Make the service nullable so constructor can handle missing ServiceProvider gracefully.
        private readonly IClaimService? _claimService;

        public HomePage()
        {
            InitializeComponent();
            var scope = App.ServiceProvider?.CreateScope();
            if (scope != null)
            {
                _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            }
            Loaded += HomePage_Loaded;
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.CurrentUser != null)
            {
                WelcomeName.Text = $"{Session.CurrentUser.FirstName} {Session.CurrentUser.LastName}";
                ProfileName.Text = $"{Session.CurrentUser.FirstName} {Session.CurrentUser.LastName}";
                ProfileRole.Text = Session.CurrentUser.Role ?? "User";
                ProfileEmail.Text = Session.CurrentUser.Email ?? "";
            }
            else
            {
                WelcomeName.Text = "Welcome";
                ProfileName.Text = "No user selected";
            }

            LoadRecentClaims();
            LoadStats();
        }

        private void LoadRecentClaims()
        {
            if (Session.CurrentUser == null || _claimService == null) return;
            try
            {
                var list = _claimService.GetClaimsForUser(Session.CurrentUser.UserId).Take(5).ToList();
                RecentClaimsGrid.ItemsSource = list;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error loading recent claims: " + ex.Message);
            }
        }

        private void LoadStats()
        {
            if (Session.CurrentUser == null || _claimService == null) return;
            try
            {
                var all = _claimService.GetClaimsForUser(Session.CurrentUser.UserId).ToList();
                // <- fix below if necessary
                StatSubmitted.Text = all.Count.ToString();
                StatPending.Text = all.Count(c => c.Status == "Submitted" || c.Status == "Reviewed").ToString();
            }
            catch { /* ignore */ }
        }

        private void GoLecturer_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new LecturerPage());
        private void GoCoordinator_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new CoordinatorPage());
        private void GoManager_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ManagerPage());
        private void GoClaimForm_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ClaimFormPage());

        private void ViewClaim_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag != null && int.TryParse(b.Tag.ToString(), out int id))
            {
                NavigationService?.Navigate(new ClaimDetailsPage(id));
            }
        }
    }
}
