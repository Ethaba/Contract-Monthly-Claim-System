using CMCS.Data;
using CMCS.Models;
using CMCS.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CMCS.Tests
{
    // Fake file service for test purposes (does not write files)
    class FakeFileService : IFileService
    {
        public long MaxFileSizeBytes => 10 * 1024 * 1024;
        public bool IsAllowedExtension(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant() ?? string.Empty;
            return ext == ".pdf" || ext == ".docx" || ext == ".xlsx";
        }

        public (bool Success, string? Error, string? StoredFilePath) SaveFile(string sourcePath, int claimId)
        {
            return (true, null, $"/fake/{claimId}/{Path.GetFileName(sourcePath)}");
        }
    }

    public class ClaimServiceTests : IDisposable
    {
        private readonly DbContextOptions<AppDbContext> _opts;
        private readonly AppDbContext _db;

        public ClaimServiceTests()
        {
            _opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("cmcs_test_db_" + Guid.NewGuid())
                .Options;
            _db = new AppDbContext(_opts);
            _db.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        // ADAPT THIS METHOD if your ClaimService constructor is different
        private ClaimService CreateService()
        {
            var fileService = new FakeFileService();
            // common constructor: ClaimService(AppDbContext db, IFileService fileService)
            // If your ClaimService signature is different, adjust here accordingly.
            return new ClaimService(_db, fileService);
        }

        [Fact]
        public void CreateClaim_ComputesTotalAndSavesItems()
        {
            // Arrange
            var user = new User { Username = "test_lect", FirstName = "Test", LastName = "User", Role = "Lecturer" };
            _db.Users.Add(user);
            _db.SaveChanges();

            var svc = CreateService();

            var claim = new Claim
            {
                UserId = user.UserId,
                ClaimMonth = 9,
                ClaimYear = 2025,
                Status = "Submitted"
            };

            var items = new List<ClaimItem>
            {
                new ClaimItem { Description = "Item A", HoursWorked = 2m, HourlyRate = 100m },
                new ClaimItem { Description = "Item B", HoursWorked = 1.5m, HourlyRate = 80m }
            };

            // Act
            var created = svc.CreateClaim(claim, items);

            // Assert - reload from db
            var fromDb = _db.Claims.Include(c => c.ClaimItems).First(c => c.ClaimId == created.ClaimId);
            Assert.Equal(320m, fromDb.TotalAmount); // 200 + 120
            Assert.Equal(2, fromDb.ClaimItems.Count);
        }

        [Fact]
        public void ApproveClaim_SetsStatusAndRecordsApproval()
        {
            // Arrange
            var user = new User { Username = "test_lect2", FirstName = "T2", Role = "Lecturer" };
            _db.Users.Add(user);
            _db.SaveChanges();

            var claim = new Claim
            {
                UserId = user.UserId,
                ClaimMonth = 6,
                ClaimYear = 2025,
                Status = "Submitted",
                TotalAmount = 100m
            };
            _db.Claims.Add(claim);
            _db.SaveChanges();

            var svc = CreateService();

            // Act
            svc.ApproveClaim(claim.ClaimId, approverUserId: user.UserId, role: "Coordinator", decision: "Approved", comments: "OK");

            // Assert
            var updated = _db.Claims.Include(c => c.Approvals).First(c => c.ClaimId == claim.ClaimId);
            Assert.Equal("Approved", updated.Status);
            Assert.Single(updated.Approvals);
            Assert.Equal("Coordinator", updated.Approvals.First().Role);
        }
    }
}
