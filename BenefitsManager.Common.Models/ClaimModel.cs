namespace BenefitsManager.Common.Models
{
    public class ClaimModel
    {
        public required Guid ClaimId { get; set; }
        public required string Merchant { get; set; }
        public decimal ClaimedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public long PurchaseDate { get; set; }
        public required ClaimCategoryModel Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public required List<ClaimStatusModel> StatusLog { get; set; }
        public required ClaimStatus CurrentStatus { get; set; } = ClaimStatus.Pending;
        public required string ReceiptPath { get; set; }
        public required UserModel CreatedBy { get; set; }
        public long CreatedOn { get; set; }
        public long? ModifiedOn { get; set; }
        public bool IsOverDue { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ClaimCategoryModel
    {
        public required string CategoryCode { get; set; }
        public required string ParentCategoryName { get; set; }
        public required string CategoryName { get; set; }
    }

    public class ClaimStatusModel
    {
        public required ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public string? Comment { get; set; }
        public required UserModel SetBy { get; set; }
        public long Ts { get; set; }

    }

    public class UserModel
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
    }

    public class ClaimAddModel
    {
        public string Merchant { get; set; }
        public decimal ClaimedAmount { get; set; }
        public long PurchaseDate { get; set; }
        public string CategoryCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReceiptPath { get; set; }
        public UserModel CreatedBy { get; set; }
    }

    public class ClaimUpdateModel
    {
        public required string Merchant { get; set; }
        public decimal ClaimedAmount { get; set; }
        public long PurchaseDate { get; set; }
        public required string CategoryCode { get; set; }
        public string Description { get; set; } = string.Empty;
        public required string ReceiptPath { get; set; }
    }

    public class ClaimStatusUpdateModel
    {
        public required decimal ApprovedAmount { get; set; }
        public required ClaimStatus NewStatus { get; set; }
        public string Comment { get; set; } = string.Empty;
        public required UserModel SetBy { get; set; }
    }
}
