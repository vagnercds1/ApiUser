using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using FluentValidation;

namespace ApiUser.Domain.Validations;

public class UserValidationAdd : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;

    public UserValidationAdd(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x).Cascade(CascadeMode.Continue)
          .Must(x => !string.IsNullOrEmpty(x.Document)).WithMessage("It is mandatory to inform the Document")
          .Must(x => !string.IsNullOrEmpty(x.Email)).WithMessage("It is mandatory to inform the Email")
          .Must(x => !string.IsNullOrEmpty(x.Password)).WithMessage("It is mandatory to inform the Password")
          .Must(x => !string.IsNullOrEmpty(x.FullName)).WithMessage("It is mandatory to inform the FullName")
          .MustAsync(async (user, cancellation) => !await UserAlreadyExists(user.Email).ConfigureAwait(false)).WithMessage("User Already Exists");
    } 
   
    private async Task<bool> UserAlreadyExists(string email)
    {
        var result = await _userRepository.GetUserAsync(new User() { Email=email});
        return result.Any(); 
    }
}
