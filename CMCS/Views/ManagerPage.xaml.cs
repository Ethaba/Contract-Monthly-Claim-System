using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMCS.Views
{
    public partial class ManagerPage : Page
    {
        private readonly IClaimService _claimService;

        public ManagerPage()
        {
            InitializeComponent();
            var scope = App.ServiceProvider!.CreateScope();
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            Loaded += ManagerPage_Loaded;
        }

        private void ManagerPage_Loaded(object sender, RoutedEventArgs e) => LoadPending();

        private void LoadPending()
        {
            var list = _claimService.GetPendingClaims().ToList();
            DataGridPending.ItemsSource = list;
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPending.SelectedItem is Claim c)
            {
                if (Session.CurrentUser == null) { MessageBox.Show("Select a user."); return; }
                _claimService.ApproveClaim(c.ClaimId, Session.CurrentUser.UserId, "Manager", "Approved", "Approved by manager");
                MessageBox.Show("Claim approved.");
                LoadPending();
            }
            else MessageBox.Show("Select a claim first.");
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPending.SelectedItem is Claim c)
            {
                if (Session.CurrentUser == null) { MessageBox.Show("Select a user."); return; }
                _claimService.ApproveClaim(c.ClaimId, Session.CurrentUser.UserId, "Manager", "Rejected", "Rejected by manager");
                MessageBox.Show("Claim rejected.");
                LoadPending();
            }
            else MessageBox.Show("Select a claim first.");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadPending();

        private void Home_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new HomePage());
    }
}
