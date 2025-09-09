using System.Windows;

namespace CMCS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LecturerDashboard_Click(object sender, RoutedEventArgs e)
        {
            LecturerDashboard lecturerDashboard = new LecturerDashboard();
            lecturerDashboard.Show();
        }

        private void CoordinatorDashboard_Click(object sender, RoutedEventArgs e)
        {
            CoordinatorDashboard coordinatorDashboard = new CoordinatorDashboard();
            coordinatorDashboard.Show();
        }

        private void ManagerDashboard_Click(object sender, RoutedEventArgs e)
        {
            ManagerDashboard managerDashboard = new ManagerDashboard();
            managerDashboard.Show();
        }

        private void ClaimForm_Click(object sender, RoutedEventArgs e)
        {
            ClaimForm claimForm = new ClaimForm();
            claimForm.Show();
        }
    }
}
