namespace courierMs.Models
{
    public class Helper
    {
    }
    public static class LookupType
    {
        public const string City = "City";
        public const string Percel = "Percel";
    }
    public static class RoleType
    {
        public const string Customer = "Customer";
        public const string Admin = "Admin";
        public const string S_Admin = "SuperAdmin";
    }
    public static class PopupMessage
    {
        public const string success = "Good Job";
        public const string error = "failed";
        public const string notValid = "User already exist with this number";

    }

    public static class GuidHelper
    {
        public static Guid ToGuidOrDefault(string guidString)
        {
            return !string.IsNullOrEmpty(guidString) && Guid.TryParse(guidString, out Guid guid)
                ? guid
                : Guid.Empty;
        }
    }
}
