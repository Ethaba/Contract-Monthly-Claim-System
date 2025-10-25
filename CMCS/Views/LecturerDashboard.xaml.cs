using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;

namespace CMCS.Views
{
    public partial class LecturerDashboard : Window
    {
        private readonly IClaimService _claimService;

        public LecturerDashboard()
        {
            InitializeComponent();
            var scope = App.ServiceProvider?.CreateScope();
            if (scope == null) throw new InvalidOperationException("Service provider not configured.");
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();

            this.Loaded += LecturerDashboard_Loaded;
        }

        private void LecturerDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            LoadClaims();
        }

        private void LoadClaims()
        {
            try
            {
                var user = Session.CurrentUser;
                if (user == null)
                {
                    MessageBox.Show("Select a user in Main window first.");
                    return;
                }

                var list = _claimService.GetClaimsForUser(user.UserId).ToList();
                DataGridClaims.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading claims: " + ex.Message);
            }
        }

        private void SubmitNewClaim_Click(object sender, RoutedEventArgs e)
        {
            var form = new ClaimForm();
            form.ShowDialog();
            LoadClaims();
        }

        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Upload documents via the Claim Form after creating a claim.");
        }
    }
}
