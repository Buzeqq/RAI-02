using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RAI_02.Dto;
using RAI_02.Repositories;

namespace RAI_02.Controllers;

public class LoginController : Controller
{
    private readonly IUserRepository _userRepository;

    public LoginController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("/Login/{login}")]
    public IActionResult Login(string login)
    {
        var userSession = new UserSessionInfomation { Login = login };

        if (_userRepository.Get(login) is null)
        {
            ViewData["userNotFound"] = true;
            return View("Index");
        }
        
        var serialized = JsonSerializer.Serialize(userSession);
        HttpContext.Session.SetString("session", serialized);
        return RedirectToAction("Index", "Friends");
    }

    [HttpGet]
    [Route("/Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}
