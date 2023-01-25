using WebAutoTest.Models;
using WebAutoTest.Repositories;

namespace WebAutoTest.Services;

public class UserService
{
    private readonly CookieService _cookieService;
    private readonly UsersRepository _usersRepository;

    public UserService(CookieService cookieService, UsersRepository usersRepository)
    {
        _cookieService = cookieService;
        _usersRepository = usersRepository;
    }



    public User? GetUserFromCookie(HttpContext context)
    {
        var userPhone = _cookieService.GetUserPhoneFromCookie(context);
        if (userPhone != null)
        {
            var user = _usersRepository.GetUsersByPhoneNumber(userPhone);
            if (user.PhoneNumber == userPhone)
            {
                return user;
            }
        }
        return null;
    }
}
