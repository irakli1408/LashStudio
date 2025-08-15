using FluentValidation;
using MediatR;

namespace LashStudio.Application.Common.Behaviors;

public sealed class ValidationBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
    where TReq : notnull
{
    private readonly IEnumerable<IValidator<TReq>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TReq>> validators) => _validators = validators;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (_validators.Any())
        {
            var ctx = new ValidationContext<TReq>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(ctx, ct)));
            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0) throw new ValidationException(failures);
        }

        return await next();
    }
}
