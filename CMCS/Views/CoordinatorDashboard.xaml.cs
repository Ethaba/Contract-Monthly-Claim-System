


using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;

namespace CMCS.Views
{
    public partial class CoordinatorDashboard : Window
    {
        private readonly IClaimService _claimService;

        public CoordinatorDashboard()
        {
            InitializeComponent();
            var scope = App.ServiceProvider?.CreateScope();
            if (scope == null) throw new InvalidOperationException("Service provider not configured.");
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            this.Loaded += CoordinatorDashboard_Loaded;
        }

        private void CoordinatorDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPending();
        }

        private void LoadPending()
        {
            var list = _claimService.GetPendingClaims().ToList();
            DataGridPending.ItemsSource = list;
        }

        private void VerifyClaim_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPending.SelectedItem is Claim c)
            {
                var user = Session.CurrentUser;
                if (user == null) { MessageBox.Show("Select user in Main window"); return; }

                try
                {
                    _claimService.ApproveClaim(c.ClaimId, user.UserId, "Coordinator", "Approved", "Verified");
                    MessageBox.Show("Claim verified.");
                    LoadPending();
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
            else MessageBox.Show("Select a claim first.");
        }

        private void RequestRevision_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPending.SelectedItem is Claim c)
            {
                var user = Session.CurrentUser;
                if (user == null) { MessageBox.Show("Select user in Main window"); return; }

                _claimService.ApproveClaim(c.ClaimId, user.UserId, "Coordinator", "Rejected", "Please revise");
                MessageBox.Show("Revision requested.");
                LoadPending();
            }
            else MessageBox.Show("Select a claim first.");
        }
    }
}
