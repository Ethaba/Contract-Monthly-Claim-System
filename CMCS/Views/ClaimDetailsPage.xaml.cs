using CMCS.Data;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CMCS.Views
{
    public partial class ClaimDetailsPage : Page
    {
        private readonly IClaimService _claimService;
        private readonly int _claimId;

        public ClaimDetailsPage(int claimId)
        {
            InitializeComponent();
            _claimId = claimId;
            var scope = App.ServiceProvider!.CreateScope();
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            Loaded += ClaimDetailsPage_Loaded;
        }

        private void ClaimDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var claim = _claimService.GetClaimWithDetails(_claimId);
            if (claim == null) { MessageBox.Show("Claim not found."); NavigationService?.Navigate(new LecturerPage()); return; }

            HeaderText.Text = $"Claim #{claim.ClaimId}";
            MetaText.Text = $"Lecturer: {claim.User?.FirstName} {claim.User?.LastName}  • Month: {claim.ClaimMonth}/{claim.ClaimYear}  • Status: {claim.Status}";
            ItemsGrid.ItemsSource = claim.ClaimItems.ToList();
            DocsList.ItemsSource = claim.SupportingDocuments.Select(d => d.FileName).ToList();
        }

        private void Back_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();
    }
}
