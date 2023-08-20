using CartEase.Application.Domain;
using FluentValidation;

namespace CartEase.Application.Validators;

public class ItemImageValidator : AbstractValidator<ItemImage>
{
    public ItemImageValidator()
    {
        RuleFor(image => image.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(100).WithMessage("File name must not exceed 100 characters.");

        RuleFor(image => image.FileBytes)
            .NotEmpty().WithMessage("File bytes are required.");

        RuleFor(image => image.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .MaximumLength(50).WithMessage("Content type must not exceed 50 characters.")
            .Must(BeValidImageContentType).WithMessage("Invalid image content type. Supported types are JPEG, PNG, and GIF.");

        RuleFor(image => image.ContentDisposition)
            .MaximumLength(100).WithMessage("Content disposition must not exceed 100 characters.");

        RuleFor(image => image.Length)
            .GreaterThan(0).WithMessage("Length must be greater than 0.");

        RuleFor(image => image.Name)
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
    }

    private bool BeValidImageContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        // List of common image content types
        var validContentTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/gif"
        };

        return validContentTypes.Contains(contentType);
    }
}