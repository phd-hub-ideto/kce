using KhumaloCraft.Application.Portal.Models;
using KhumaloCraft.Application.Portal.Models.Home;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Domain.Team;
using KhumaloCraft.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KhumaloCraft.Application.Portal.Controllers;

public sealed class HomeController : BaseController
{
    private readonly ITeamService _teamService;
    private readonly IUserRepository _userRepository;

    public HomeController(ITeamService teamService, IUserRepository userRepository)
    {
        _teamService = teamService;
        _userRepository = userRepository;
    }

    [Route("/", Name = RouteNames.Site.Home)]
    public IActionResult Index()
    {
        return View();
    }

    [Route("about")]
    public IActionResult AboutUs()
    {
        var teamMembers = _teamService.GetAllTeamMembers().Select(teamMember =>
        {
            return new TeamMemberModel
            {
                Id = teamMember.Id,
                Name = teamMember.Name,
                Role = teamMember.Role,
                EmailAddress = teamMember.EmailAddress,
                ImageUrl = teamMember.ImageUrl,
            };
        });

        //If this executes and we see the about us page, it means the db connections works fine. ELse we should see an error (ErrorPage)
        var user = _userRepository.Query().SingleOrDefault(u => u.Id == 1);

        return View(teamMembers);
    }

    [Route("contact")]
    public IActionResult ContactUs(ContactUsModel model)
    {
        ViewBag.Message = TempData["Message"];

        return View(model);
    }

    [HttpPost]
    [Route("contact")]
    public IActionResult SubmitMessage([FromForm] ContactUsModel model)
    {
        if (ModelState.IsValid)
        {
            TempData["Message"] = "Thank you for reaching out, " + model.Name + ". We'll get back to you soon!";

            return RedirectToAction("ContactUs");
        }

        return View("ContactUs", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}