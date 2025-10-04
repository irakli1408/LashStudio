using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Courses;
using LashStudio.Domain.Faq;
using LashStudio.Domain.Services;

namespace LashStudio.Application.Handlers.Admin.Commands.Publish.Common.Helper
{
    public sealed class SetActiveCourseHandler
    : SetActiveHandler<Course, long>
    {
        public SetActiveCourseHandler(IAppDbContext db) : base(db) { }
    }

    public sealed class SetActiveServiceHandler
        : SetActiveHandler<Service, Guid>
    {
        public SetActiveServiceHandler(IAppDbContext db) : base(db) { }
    }

    // при необходимости добавляй сюда другие сущности:
    public sealed class SetActiveFaqHandler
        : SetActiveHandler<FaqItem, long>
    {
        public SetActiveFaqHandler(IAppDbContext db) : base(db) { }
    }
}
