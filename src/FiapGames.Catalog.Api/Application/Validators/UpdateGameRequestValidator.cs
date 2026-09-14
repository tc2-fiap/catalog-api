using FiapGames.Catalog.Api.Application.Dtos;
using FluentValidation;

namespace FiapGames.Catalog.Api.Application.Validators;

public sealed class UpdateGameRequestValidator : AbstractValidator<UpdateGameRequest>
{
    public UpdateGameRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Platform).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).LessThanOrEqualTo(999_999m);
        RuleFor(x => x.ReleaseDate)
            .GreaterThanOrEqualTo(new DateOnly(1970, 1, 1))
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5)));
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.CoverImageUrl).MaximumLength(2048);
        RuleFor(x => x.CoverImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https")
            .When(x => x.CoverImageUrl is not null)
            .WithMessage("CoverImageUrl must be a valid absolute http(s) URL.");
    }
}
