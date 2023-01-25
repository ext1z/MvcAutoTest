using Microsoft.AspNetCore.Mvc;
using WebAutoTest.Models;
using WebAutoTest.Repositories;
using WebAutoTest.Services;

namespace WebAutoTest.Controllers;


[Route("[action]")]
public class UserController : Controller
{
    private readonly UsersRepository _usersRepository;
    private readonly TicketsRepository _ticketsRepository;
    private readonly QuestionsRepository _questionsRepository;
    private readonly CookieService _cookieService;
    private readonly UserService _userService;


    public UserController(UsersRepository usersRepository,
                          TicketsRepository ticketsRepository,
                          QuestionsRepository questionsRepository, 
                          CookieService cookieService, 
                          UserService userService)
    {
        _usersRepository = usersRepository;
        _ticketsRepository = ticketsRepository;
        _questionsRepository = questionsRepository;
        _cookieService = cookieService;
        _userService = userService;
    }

    public IActionResult Index()
    {
        var user = _userService.GetUserFromCookie(HttpContext);

        if (user != null)
        {
            return View(user);
        }

        return RedirectToAction("SignIn");
    }

    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignUp(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        _usersRepository.InsertUser(user);
        var getUserPhone = _usersRepository.GetUsersByPhoneNumber(user.PhoneNumber);

        _ticketsRepository.InsertUserTrainingTickets(getUserPhone.Index, _questionsRepository.GetQuestionsCount() / 20, 20);
        _cookieService.SendUserPhoneToCookie(getUserPhone.PhoneNumber!, HttpContext);
        return RedirectToAction("Index");
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SignIn(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var _user = _usersRepository.GetUsersByPhoneNumber(user.PhoneNumber);

        if (_user.Password == user.Password)
        {
            _cookieService.SendUserPhoneToCookie(user.PhoneNumber!, HttpContext);
            return RedirectToAction("Index");
        }

        return RedirectToAction("SignIn");
    }
}
