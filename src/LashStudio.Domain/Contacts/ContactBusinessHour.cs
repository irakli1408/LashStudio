namespace LashStudio.Domain.Contacts
{
    public class ContactBusinessHour
    {
        public long Id { get; set; }
        public long ContactProfileId { get; set; }
        public ContactProfile ContactProfile { get; set; } = default!;

        public DayOfWeek Day { get; set; }         // 0..6
        public TimeOnly? Open { get; set; }        // могут быть null, если IsClosed или расписание не задано
        public TimeOnly? Close { get; set; }
        public bool IsClosed { get; set; } = false;
    }
}
