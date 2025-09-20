namespace LashStudio.Domain.Contacts
{
    public class ContactMessage
    {
        public long Id { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; } = default!;

        public string? ClientIp { get; set; }
        public bool ConsentAccepted { get; set; }
        public ContactMessageStatus Status { get; set; } = ContactMessageStatus.New;
    }
}
