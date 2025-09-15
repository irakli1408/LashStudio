using System.Net;

namespace LashStudio.Application.Exceptions
{
    public abstract class AppException : Exception
    {
        public string Code { get; }                 // машинный код ошибки: "course_not_found"
        public HttpStatusCode Status { get; }       // какой статус отдать API-слою
        public IReadOnlyDictionary<string, string[]>? Details { get; }

        protected AppException(
            string message,
            string code,
            HttpStatusCode status,
            IReadOnlyDictionary<string, string[]>? details = null,
            Exception? inner = null)
            : base(message, inner)
        {
            Code = code;
            Status = status;
            Details = details;
        }
    }

    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message = "not_found", string code = "not_found")
            : base(message, code, HttpStatusCode.NotFound) { }
    }

    // 400 Bad Request (некорректный ввод, но НЕ валидатор)
    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string message = "bad_request", string code = "bad_request",
            IReadOnlyDictionary<string, string[]>? details = null)
            : base(message, code, HttpStatusCode.BadRequest, details) { }
    }

    // 409 Conflict (уникальные ключи, состояние ресурса)
    public sealed class ConflictException : AppException
    {
        public ConflictException(string message = "conflict", string code = "conflict")
            : base(message, code, HttpStatusCode.Conflict) { }
    }

    // 401 Unauthorized (нет или неверная аутентификация)
    public sealed class UnauthorizedAppException : AppException
    {
        public UnauthorizedAppException(string message = "unauthorized", string code = "unauthorized")
            : base(message, code, HttpStatusCode.Unauthorized) { }
    }

    // 403 Forbidden (прав не хватает)
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "forbidden", string code = "forbidden")
            : base(message, code, HttpStatusCode.Forbidden) { }
    }

    // 422 Unprocessable Entity (бизнес-правила, не простой 400)
    public sealed class BusinessRuleViolationException : AppException
    {
        public BusinessRuleViolationException(
            string message = "business_rule_violation",
            string code = "business_rule_violation",
            IReadOnlyDictionary<string, string[]>? details = null)
            : base(message, code, HttpStatusCode.UnprocessableEntity, details) { }
    }

    // 422 (валидация — обёртка над FluentValidation, если хочешь свой формат)
    public sealed class AppValidationException : AppException
    {
        public AppValidationException(
            string message = "validation_failed",
            string code = "validation_failed",
            IReadOnlyDictionary<string, string[]>? errors = null)
            : base(message, code, HttpStatusCode.UnprocessableEntity, errors) { }
    }

    // 409 (EF Concurrency / ETag)
    public sealed class ConcurrencyException : AppException
    {
        public ConcurrencyException(string message = "concurrency_conflict", string code = "concurrency_conflict")
            : base(message, code, HttpStatusCode.Conflict) { }
    }

    // 429 (rate limiting / антиспам)
    public sealed class RateLimitExceededException : AppException
    {
        public RateLimitExceededException(string message = "rate_limit_exceeded", string code = "rate_limit_exceeded")
            : base(message, code, HttpStatusCode.TooManyRequests) { }
    }

    // 415 Unsupported Media Type (для загрузок)
    public sealed class UnsupportedMediaTypeAppException : AppException
    {
        public UnsupportedMediaTypeAppException(string message = "unsupported_media_type", string code = "unsupported_media_type")
            : base(message, code, HttpStatusCode.UnsupportedMediaType) { }
    }

    // 413 Payload Too Large (лимиты загрузок)
    public sealed class PayloadTooLargeException : AppException
    {
        public PayloadTooLargeException(string message = "payload_too_large", string code = "payload_too_large")
            : base(message, code, HttpStatusCode.RequestEntityTooLarge) { }
    }

    // 502/503 — внешние сервисы/интеграции (HTTP, очереди и т.п.)
    public sealed class ExternalServiceException : AppException
    {
        public ExternalServiceException(
            string message = "external_service_error",
            string code = "external_service_error")
            : base(message, code, HttpStatusCode.BadGateway) { }
    }

    public sealed class ServiceUnavailableAppException : AppException
    {
        public ServiceUnavailableAppException(
            string message = "service_unavailable",
            string code = "service_unavailable")
            : base(message, code, HttpStatusCode.ServiceUnavailable) { }
    }
}
