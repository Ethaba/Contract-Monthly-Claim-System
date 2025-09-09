using System.Windows;
using CMCS.Views; // Import the Views folder

namespace CMCS.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load Lecturer Dashboard by default
            MainContent.Content = new LecturerDashboard();

            // Event handlers for navigation buttons
            btnLecturer.Click += (s, e) => MainContent.Content = new LecturerDashboard();
            btnClaimForm.Click += (s, e) => MainContent.Content = new ClaimForm();
            btnCoordinator.Click += (s, e) => MainContent.Content = new CoordinatorDashboard();
            btnManager.Click += (s, e) => MainContent.Content = new ManagerDashboard();
        }
    }
}
