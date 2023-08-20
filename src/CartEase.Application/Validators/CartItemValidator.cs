using CartEase.Application.Domain;
using FluentValidation;

namespace CartEase.Application.Validators;

public class CartItemValidator : AbstractValidator<CartItem>
{
    public CartItemValidator()
    {
        RuleFor(item => item.Description).MaximumLength(500);
        RuleFor(item => item.Price).GreaterThan(0);
        RuleFor(item => item.Name).NotEmpty().MaximumLength(100);
        RuleFor(item => item.Quantity).GreaterThan(0);
    }
}