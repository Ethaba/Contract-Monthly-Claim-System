using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMCS.Views
{
    public partial class LecturerPage : Page
    {
        private readonly IClaimService _claimService;

        public LecturerPage()
        {
            InitializeComponent();
            var scope = App.ServiceProvider!.CreateScope();
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            Loaded += LecturerPage_Loaded;
        }

        private void LecturerPage_Loaded(object sender, RoutedEventArgs e) => LoadClaims();

        private void LoadClaims()
        {
            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Select a user first from the left.");
                return;
            }

            var list = _claimService.GetClaimsForUser(Session.CurrentUser.UserId).ToList();
            DataGridClaims.ItemsSource = list;
        }

        private void SubmitNewClaim_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClaimFormPage());
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadClaims();

        private void Home_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new HomePage());

        private void ViewClaim_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is int id)
            {
                NavigationService?.Navigate(new ClaimDetailsPage(id)); // I'll include a ClaimDetailsPage below
            }
        }
    }
}
