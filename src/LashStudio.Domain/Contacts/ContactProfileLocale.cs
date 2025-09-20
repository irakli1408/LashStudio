namespace LashStudio.Domain.Contacts
{
    public class ContactProfileLocale
    {
        public long Id { get; set; }
        public long ContactProfileId { get; set; }
        public ContactProfile ContactProfile { get; set; } = default!;

        public string Culture { get; set; } = "ru-RU";
        public string? OrganizationName { get; set; }
        public string? AddressLine { get; set; }
        public string? HowToFindUs { get; set; }
    }
}
