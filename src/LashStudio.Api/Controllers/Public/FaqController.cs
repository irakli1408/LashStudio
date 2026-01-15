using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Queries.Faq.Get;
using LashStudio.Application.Handlers.Public.Queries.Faq.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LashStudio.Api.Controllers.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/faq")]
    public sealed class FaqController : ApiControllerBase
    {
        public FaqController(ISender sender) : base(sender) { }

        [HttpGet]
        //[OutputCache(PolicyName = "public-10m-tagged")]
        public Task<List<FaqItemVm>> List(string culture)
            => Sender.Send(new GetFaqListQuery(culture));

        [HttpGet("{id:long}")]
        [OutputCache(PolicyName = "public-10m-tagged")]
        public Task<FaqItemVm> GetById(string culture, long id)
            => Sender.Send(new GetFaqByIdQuery(id, culture));
    }
}
