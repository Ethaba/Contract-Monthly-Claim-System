using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CMCS.Views
{
    public partial class ClaimForm : Window
    {
        private readonly IClaimService _claimService;
        private readonly IFileService _fileService;

        public ClaimForm()
        {
            InitializeComponent();

            var scope = App.ServiceProvider?.CreateScope();
            if (scope == null) throw new InvalidOperationException("Service provider not configured.");

            _claimService = scope.ServiceProvider.GetRequiredService<IClaimService>();
            _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
        }

        private void UploadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Documents (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx";
            var ok = dlg.ShowDialog();
            if (ok == true)
            {
                UploadFileButton.Tag = dlg.FileName;
                UploadedFileName.Text = System.IO.Path.GetFileName(dlg.FileName);
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var current = Session.CurrentUser;
                if (current == null)
                {
                    MessageBox.Show("Select a current user in Main window first.");
                    return;
                }

                if (MonthCombo.SelectedIndex < 0)
                {
                    MessageBox.Show("Select a month.");
                    return;
                }

                if (!decimal.TryParse(RateText.Text, out var rate))
                {
                    MessageBox.Show("Enter a valid hourly rate.");
                    return;
                }

                if (!decimal.TryParse(HoursText.Text, out var hours))
                {
                    MessageBox.Show("Enter valid hours.");
                    return;
                }

                var item = new ClaimItem
                {
                    Description = "Work",
                    HoursWorked = hours,
                    HourlyRate = rate
                };

                var claim = new Claim
                {
                    UserId = current.UserId,
                    ClaimMonth = MonthCombo.SelectedIndex + 1,
                    ClaimYear = DateTime.Now.Year,
                    Notes = NotesText.Text
                };

                var created = _claimService.CreateClaim(claim, new List<ClaimItem> { item });

                // handle file upload if present (Tag holds the source path)
                if (UploadFileButton.Tag is string sourcePath && !string.IsNullOrEmpty(sourcePath))
                {
                    var result = _fileService.SaveFile(sourcePath, created.ClaimId);
                    if (!result.Success)
                    {
                        MessageBox.Show("File upload error: " + (result.Error ?? "Unknown error"));
                    }
                    else if (!string.IsNullOrEmpty(result.StoredFilePath))
                    {
                        using var scope = App.ServiceProvider!.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        db.SupportingDocuments.Add(new SupportingDocument
                        {
                            ClaimId = created.ClaimId,
                            FileName = System.IO.Path.GetFileName(sourcePath),
                            FilePath = result.StoredFilePath,
                            FileType = System.IO.Path.GetExtension(sourcePath)
                        });
                        db.SaveChanges();
                    }
                }

                MessageBox.Show("Claim submitted successfully.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting claim: " + ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
