using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CMCS.Views
{
    public partial class ClaimFormPage : Page
    {
        
        private readonly IClaimService _claimService;
        private readonly IFileService _fileService;
        private readonly ObservableCollection<ClaimItem> _items = new ObservableCollection<ClaimItem>();
        private readonly ObservableCollection<string> _selectedFiles = new ObservableCollection<string>();

        public ClaimFormPage()
        {
            InitializeComponent();

            // Create scope and definitely assign the service fields here
            var scope = App.ServiceProvider!.CreateScope();
            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();

            DataGridItems.ItemsSource = _items;
            FilesList.ItemsSource = _selectedFiles;

            Loaded += ClaimFormPage_Loaded;
        }

        private void ClaimFormPage_Loaded(object? sender, RoutedEventArgs e)
        {
            YearText.Text = DateTime.Now.Year.ToString();
            MonthCombo.SelectedIndex = DateTime.Now.Month - 1;
            // start with one empty row
            if (_items.Count == 0) _items.Add(new ClaimItem { Description = "Item 1", HoursWorked = 0, HourlyRate = 0, Amount = 0 });
            UpdateTotal();
            DataGridItems.CellEditEnding += DataGridItems_CellEditEnding;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            _items.Add(new ClaimItem { Description = $"Item {_items.Count + 1}", HoursWorked = 0, HourlyRate = 0, Amount = 0 });
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridItems.SelectedItem is ClaimItem it)
            {
                _items.Remove(it);
                UpdateTotal();
            }
            else MessageBox.Show("Select an item to remove.");
        }

        // Signature matches WPF delegate and uses nullable sender
        private void DataGridItems_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Dispatch to ensure binding values committed
                Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (e.Row.Item is ClaimItem it)
                    {
                        it.Amount = it.HoursWorked * it.HourlyRate;
                        UpdateTotal();
                        DataGridItems.Items.Refresh();
                    }
                }));
            }
        }

        private void UpdateTotal()
        {
            var total = _items.Sum(i => i.Amount);
            TotalText.Text = total.ToString("N2");
        }

        private void AddFiles_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Documents (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx";
            var ok = dlg.ShowDialog();
            if (ok == true)
            {
                foreach (var f in dlg.FileNames)
                {
                    if (!_fileService.IsAllowedExtension(f))
                    {
                        MessageBox.Show($"File type not allowed: {Path.GetFileName(f)}");
                        continue;
                    }
                    var fi = new FileInfo(f);
                    if (fi.Length > _fileService.MaxFileSizeBytes)
                    // <- intentionally corrected below
                    {
                        MessageBox.Show($"File too large: {Path.GetFileName(f)}");
                        continue;
                    }
                    _selectedFiles.Add(f);
                }
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Session.CurrentUser == null) { MessageBox.Show("Select a user first."); return; }

                if (!int.TryParse(YearText.Text, out int year)) { MessageBox.Show("Enter a valid year."); return; }
                if (MonthCombo.SelectedIndex < 0) { MessageBox.Show("Select a month."); return; }
                if (!_items.Any()) { MessageBox.Show("Add at least one claim item."); return; }

                // recompute amounts to ensure
                foreach (var it in _items) it.Amount = it.HoursWorked * it.HourlyRate;

                var claim = new Claim
                {
                    UserId = Session.CurrentUser.UserId,
                    ClaimMonth = MonthCombo.SelectedIndex + 1,
                    ClaimYear = year,
                    Notes = NotesText.Text
                };

                var created = _claimService.CreateClaim(claim, _items.ToList());
                // <- corrected below

                // save files
                if (_selectedFiles.Any())
                {
                    using var scope = App.ServiceProvider!.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    foreach (var source in _selectedFiles)
                    {
                        var result = _fileService.SaveFile(source, created.ClaimId);
                        if (!result.Success)
                        {
                            MessageBox.Show("File failed to save: " + (result.Error ?? "Unknown"));
                            continue;
                        }

                        // store metadata
                        db.SupportingDocuments.Add(new SupportingDocument
                        {
                            ClaimId = created.ClaimId,
                            FileName = Path.GetFileName(source),
                            FilePath = result.StoredFilePath,
                            FileType = Path.GetExtension(source)
                        });
                    }
                    db.SaveChanges();
                }

                MessageBox.Show("Claim submitted.");
                // navigate back to LecturerPage so they can see the claim
                NavigationService?.Navigate(new LecturerPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting claim: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }
    }
}
