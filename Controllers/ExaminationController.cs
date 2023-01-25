using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebAutoTest.ApplicationOptions;
using WebAutoTest.Models;
using WebAutoTest.Repositories;
using WebAutoTest.Services;

namespace WebAutoTest.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly QuestionsRepository _questionsRepository;
        private readonly TicketsRepository _ticketsRepository;
        private readonly UserService _userService;
        private readonly int TicketQuestionCount = 20;

        public ExaminationController(QuestionsRepository questionsRepository,
                                     TicketsRepository ticketsRepository, 
                                     UserService userService,
                                     IOptions<TicketSettings> option)
        {
            _questionsRepository = questionsRepository;
            _ticketsRepository = ticketsRepository;
            _userService = userService;
            TicketQuestionCount = option.Value.QuestionsCount;
        }

        public IActionResult Index()
        {
            var user = _userService.GetUserFromCookie(HttpContext);

            if (user == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            var ticket = AddRandomTicket(user);
            return View(ticket);
        }


        private Ticket AddRandomTicket(User user)
        {
            var ticketCount = _questionsRepository.GetQuestionsCount() / TicketQuestionCount;
            var random = new Random();
            var ticketIndex = random.Next(0, ticketCount);
            var from = ticketIndex * TicketQuestionCount;
            var ticket  = new Ticket(user.Index, from, TicketQuestionCount);

            _ticketsRepository.InsertTicket(ticket);

            var id = _ticketsRepository.GetLastRowId();
            ticket.Id = id;

            return ticket;
        }


        public IActionResult GetQuestionById(int questionId)
        {
            var question = _questionsRepository.GetQuestionById(questionId);
            return View(question);
        }


        [Route("tickets/{ticketId}")]
        [Route("tickets/{ticketId}/questions/{questionId}")]
        [Route("tickets/{ticketId}/questions/{questionId}/choices/{choiceId}")]
        public IActionResult Exam(int ticketId, int? questionId = null, int? choiceId = null)
        {
            var user = _userService.GetUserFromCookie(HttpContext);

            if (user == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            var ticket = _ticketsRepository.GetTicketById(ticketId, user.Index);
            questionId = questionId ?? ticket.FromIndex;
            

            if (ticket.FromIndex <= questionId && ticket.QuestionsCount + ticket.FromIndex > questionId)
            {
                ViewBag.Ticket = ticket;
                var question = _questionsRepository.GetQuestionById(questionId.Value);

                ViewBag.TicketData = _ticketsRepository.GetTicketDataById(ticket.Id);

                var _ticketData = _ticketsRepository.GetTicketDataByQuestionId(ticketId, questionId.Value);
                var _choiceId = (int?)null;
                var _answer = false;

                if (_ticketData != null)
                {
                    _choiceId = _ticketData.ChoiceId;
                    _answer = _ticketData.Answer;
                }
                else if (choiceId != null)
                {
                    var answer = question.Choices!.First(choice => choice.Id == choiceId).Answer;
                    var ticketData = new TicketData()
                    {
                        TicketId = ticketId,
                        QuestionId = question.Id,
                        ChoiceId = choiceId.Value,
                        Answer = answer
                        
                    };
                    _ticketsRepository.InsertTicketData(ticketData);

                    _choiceId = choiceId;
                    _answer = answer;

                    if (_answer)
                    {
                        _ticketsRepository.UpdateTicketCorrectCount(ticket.Id);
                    }

                    if (ticket.QuestionsCount == _ticketsRepository.GetTicketAnswerCount(ticket.Id))
                    {
                        return RedirectToAction("ExamResult", new {ticketId = ticket.Id});
                    }
                }
                ViewBag.ChoiceId = _choiceId;
                ViewBag.Answer = _answer;

                return View(question);
            }
            return NotFound();
        }

        public IActionResult ExamResult(int ticketId)
        {
            var user = _userService.GetUserFromCookie(HttpContext);

            if (user == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            var ticket = _ticketsRepository.GetTicketById(ticketId, user.Index);
            return View(ticket);
        }
    }
}
