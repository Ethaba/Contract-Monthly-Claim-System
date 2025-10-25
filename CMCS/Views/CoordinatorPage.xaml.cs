using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMCS.Views
{
    public partial class CoordinatorPage : Page
    {
        private readonly IClaimService _claimService;

        public CoordinatorPage()
        {
            InitializeComponent();
            var scope = App.ServiceProvider!.CreateScope();
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            Loaded += CoordinatorPage_Loaded;
        }

        private void CoordinatorPage_Loaded(object sender, RoutedEventArgs e) => LoadPending();

        private void LoadPending()
        {
            var list = _claimService.GetPendingClaims().ToList();
            DataGridPending.ItemsSource = list;
        }

        private void VerifyClaim_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPending.SelectedItem is Claim c)
            {
                if (Session.CurrentUser == null) { MessageBox.Show("Select a user."); return; }
                _claimService.ApproveClaim(c.ClaimId, Session.CurrentUser.UserId, "Coordinator", "Approved", "Verified by coordinator");
                MessageBox.Show("Claim verified.");
                LoadPending();
            }
            else MessageBox.Show("Select a claim first.");
        }

        private void RequestRevision_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPending.SelectedItem is Claim c)
            {
                if (Session.CurrentUser == null) { MessageBox.Show("Select a user."); return; }
            
                _claimService.ApproveClaim(c.ClaimId, Session.CurrentUser.UserId, "Coordinator", "Rejected", "Please revise");
                MessageBox.Show("Revision requested.");
                LoadPending();
            }
            else MessageBox.Show("Select a claim first.");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadPending();

        private void Home_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new HomePage());
    }
}
