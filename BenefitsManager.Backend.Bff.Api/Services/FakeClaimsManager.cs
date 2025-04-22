using BenefitsManager.Common.Models;

namespace BenefitsManager.Backend.Bff.Api.Services
{
    public class FakeClaimsManager : IClaimsManager
    {
        private readonly List<ClaimModel> _claimsList = new List<ClaimModel>();
        private readonly List<UserModel> _usersList = new List<UserModel>();
        private readonly List<ClaimCategoryModel> _claimCategoriesList = new List<ClaimCategoryModel>();

        public FakeClaimsManager()
        {
            GenerateRandomUsers();
            GenerateRandomCategories();
            GenerateRandomClaims();
        }

        private void GenerateRandomClaims()
        {
            for (int i = 0; i < 10; i++)
            {
                var randomNo = Random.Shared.Next(1, 6);

                var claimId = Guid.NewGuid();

                var claim = new ClaimModel
                {
                    ClaimId = claimId,
                    Merchant = $"Merchant-{i}",
                    ClaimedAmount = i * 10,
                    PurchaseDate = DateTimeOffset.Now.AddDays(-randomNo).ToUnixTimeMilliseconds(),
                    Category = _claimCategoriesList.First(c => c.CategoryCode == $"CAT00{randomNo}"),
                    Description = "Random description" + randomNo,
                    StatusLog = new List<ClaimStatusModel>
                    {
                        new ClaimStatusModel
                        {
                            Status = ClaimStatus.Pending,
                            Comment = "",
                            SetBy = _usersList.First(u=>u.Email == $"user{randomNo}@mail.com") ,
                            Ts = DateTimeOffset.Now.AddHours(-randomNo).ToUnixTimeMilliseconds()
                        }
                    },
                    CurrentStatus = ClaimStatus.Pending,
                    ReceiptPath = $"https://storage.blob.core.windows.net/claims/{claimId}/receipt.pdf",
                    CreatedBy = _usersList.First(u => u.Email == $"user{randomNo}@mail.com"),
                    ApprovedAmount = null,
                    CreatedOn = DateTimeOffset.Now.AddHours(-randomNo).ToUnixTimeMilliseconds(),
                    ModifiedOn = null
                };

                _claimsList.Add(claim);
            }
        }

        private void GenerateRandomUsers()
        {
            for (int i = 1; i < 6; i++)
            {
                var user = new UserModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = $"user{i}@mail.com",
                    Name = $"User-{i}"
                };
                _usersList.Add(user);
            }
        }

        private void GenerateRandomCategories()
        {

            var categories = new List<ClaimCategoryModel>
            {
                new ClaimCategoryModel { CategoryCode = "CAT001", ParentCategoryName = "Clothing and shoes", CategoryName = "Athletic Accessories" },
                new ClaimCategoryModel { CategoryCode = "CAT002", ParentCategoryName = "Clothing and shoes", CategoryName = "Athletic Apparel" },
                new ClaimCategoryModel { CategoryCode = "CAT003", ParentCategoryName = "Fitness Activities", CategoryName = "Gym" },
                new ClaimCategoryModel { CategoryCode = "CAT004", ParentCategoryName = "Fitness Activities", CategoryName = "Fitness Classes" },
                new ClaimCategoryModel { CategoryCode = "CAT005", ParentCategoryName = "Home Office", CategoryName = "Desks and Chairs" }
            };

            _claimCategoriesList.AddRange(categories);

        }
        public Task<List<ClaimModel>> GetClaimsByCreatorAsync(string userId)
        {
            var claimsList = _claimsList.Where(t => t.CreatedBy.Email.Equals(userId)).OrderByDescending(o => o.CreatedOn).ToList();
            return Task.FromResult(claimsList);
        }

        public Task<ClaimModel?> GetClaimByIdAsync(Guid claimId)
        {
            var claimModel = _claimsList.FirstOrDefault(t => t.ClaimId.Equals(claimId));
            return Task.FromResult(claimModel);
        }

        public Task<Guid> CreateNewClaimAsync(string merchant,
                                                decimal claimedAmount,
                                                long purchaseDate,
                                                string categoryCode,
                                                string description,
                                                string receiptPath,
                                                UserModel createdBy)
        {

            var claimModel = new ClaimModel()
            {
                ClaimId = Guid.NewGuid(),
                Merchant = merchant,
                ClaimedAmount = claimedAmount,
                PurchaseDate = purchaseDate,
                Category = _claimCategoriesList.First(c => c.CategoryCode == categoryCode),
                Description = description,
                StatusLog = new List<ClaimStatusModel>
            {
                new ClaimStatusModel
                {
                    Status = ClaimStatus.Pending,
                    Comment = "",
                    SetBy = createdBy,
                    Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                }
            },
                CurrentStatus = ClaimStatus.Pending,
                ReceiptPath = receiptPath,
                CreatedBy = createdBy,
                CreatedOn = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ModifiedOn = null
            };

            _claimsList.Add(claimModel);

            return Task.FromResult(claimModel.ClaimId);
        }

        public Task<bool> UpdateClaimAsync(Guid claimId,
                                            string merchant,
                                            decimal claimedAmount,
                                            long purchaseDate,
                                            string categoryCode,
                                            string description,
                                            string receiptPath)
        {
            var claimModel = _claimsList.FirstOrDefault(t => t.ClaimId.Equals(claimId));

            if (claimModel == null)
            {
                return Task.FromResult(false);
            }

            claimModel.Merchant = merchant;
            claimModel.ClaimedAmount = claimedAmount;
            claimModel.PurchaseDate = purchaseDate;
            claimModel.Category = _claimCategoriesList.First(c => c.CategoryCode == categoryCode);
            claimModel.Description = description;
            claimModel.ReceiptPath = receiptPath;
            claimModel.ModifiedOn = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            return Task.FromResult(true);
        }

        public Task<bool> UpdateClaimStatusAsync(Guid claimId,
                                                    decimal approvedAmount,
                                                    ClaimStatus newStatus,
                                                    string comment,
                                                    UserModel setBy)
        {
            var claimModel = _claimsList.FirstOrDefault(t => t.ClaimId.Equals(claimId));

            if (claimModel == null)
            {
                return Task.FromResult(false);
            }

            claimModel.ApprovedAmount = approvedAmount;

            claimModel.StatusLog.Add(new ClaimStatusModel
            {
                Status = newStatus,
                Comment = comment,
                SetBy = setBy,
                Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            });

            claimModel.CurrentStatus = newStatus;
            claimModel.ModifiedOn = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            return Task.FromResult(true);
        }

        public Task<bool> DeleteClaimAsync(Guid claimId)
        {
            var claimModel = _claimsList.FirstOrDefault(t => t.ClaimId.Equals(claimId) && t.CurrentStatus == ClaimStatus.Pending);

            if (claimModel == null)
            {
                return Task.FromResult(false);
            }

            _claimsList.Remove(claimModel);

            return Task.FromResult(true);
        }

        public Task MarkOverdueClaims(List<ClaimModel> overdueClaimsList)
        {
            throw new NotImplementedException();
        }

        public Task<List<ClaimModel>> GetYesterdaysDueClaims()
        {
            var claimList = _claimsList.Where(c => c.IsOverDue).ToList();

            return Task.FromResult(claimList);
        }
    }
}