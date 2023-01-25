using Microsoft.AspNetCore.Mvc;
using WebAutoTest.Repositories;
using WebAutoTest.Services;

namespace WebAutoTest.Controllers;

public class TicketController : Controller
{
    private readonly UserService _userService;
    private readonly TicketsRepository _ticketsRepository;

    public TicketController(UserService userService, TicketsRepository ticketsRepository )
    {
        _userService = userService;
        _ticketsRepository = ticketsRepository;
    }

    public IActionResult Index()
    {
        var user = _userService.GetUserFromCookie(HttpContext);

        if (user == null)
        {
            return RedirectToAction("SignIn", "User");
        }

        var tickets = _ticketsRepository.GetTicketsByUserId(user.Index);


        return View(tickets);
    }
}
