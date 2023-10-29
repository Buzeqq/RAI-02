using Microsoft.AspNetCore.Mvc;
using RAI_02.Dto;
using RAI_02.Models;
using RAI_02.Repositories;
using RAI_02.Services;

namespace RAI_02.Controllers;

public class UserController : Controller
{
    private readonly ISeedUsersService _seedUsersService;
    private readonly IUserRepository _userRepository;

    public UserController(ISeedUsersService seedUsersService, IUserRepository userRepository)
    {
        _seedUsersService = seedUsersService;
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult List()
    {
        return View(_userRepository.GetAll());
    }

    [HttpGet]
    public IActionResult Init()
    {
        _userRepository.Clear();
        foreach (var user in _seedUsersService.Seed())
        {
            _userRepository.Add(user);
        }

        return View("List", _userRepository.GetAll());
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Add(CreateUserDto user)
    {
        var login = user.Login;
        var newUser = new User(Guid.NewGuid(), login);
        
        _userRepository.Add(newUser);
        
        return View("List", _userRepository.GetAll());
    }

    [HttpGet]
    [Route("/User/Details/{login}")]
    public IActionResult Details(string login)
    {
        var user = _userRepository.Get(login);

        return View(user);
    }

    [HttpPost]
    [Route("/User/Delete/{login}")]
    public IActionResult Delete(string login)
    {
        var user = _userRepository.Get(login);
        if (user is null)
        {
            return View("List");
        }
        
        _userRepository.Delete(user);
        return View("List");
    }
}