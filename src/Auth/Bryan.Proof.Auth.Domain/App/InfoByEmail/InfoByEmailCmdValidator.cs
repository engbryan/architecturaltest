using FluentValidation;

namespace Bryan.Proof.Auth.Domain.App;

public class InfoByEmailCmdValidator : AbstractValidator<InfoByEmailCmd>
{
    public InfoByEmailCmdValidator()
    {
        RuleFor(p => p.Email).NotEmpty().WithMessage("Não informado");
        RuleFor(p => p.Email).EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("Email inválido");
        RuleFor(p => p.Password).NotEmpty();
    }
}