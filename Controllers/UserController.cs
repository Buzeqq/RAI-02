using System.Text.Json;
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
        if (!CanAccess(HttpContext))
        {
            return RedirectToAction("AccessDenied"); 
        } 
        
        return View(_userRepository.GetAll());
    }

    [HttpGet]
    public IActionResult Init()
    {
        if (!CanAccess(HttpContext))
        {
            return RedirectToAction("AccessDenied"); 
        } 
        
        _userRepository.Clear();
        foreach (var user in _seedUsersService.Seed())
        {
            _userRepository.Add(user);
        }

        return RedirectToAction("List");
    }

    [HttpGet]
    public IActionResult Add()
    {
        if (!CanAccess(HttpContext))
        {
            return RedirectToAction("AccessDenied"); 
        } 
        
        return View();
    }

    [HttpPost]
    public IActionResult Add(CreateUserDto user)
    {
        if (!CanAccess(HttpContext))
        {
            return RedirectToAction("AccessDenied"); 
        } 
        
        var login = user.Login;
        var newUser = new User(Guid.NewGuid(), login);
        
        _userRepository.Add(newUser);
        
        return RedirectToAction("List");
    }

    [HttpGet]
    [Route("/User/Details/{login}")]
    public IActionResult Details(string login)
    {
        if (!CanAccess(HttpContext))
        {
            return RedirectToAction("AccessDenied"); 
        } 
        
        var user = _userRepository.Get(login);

        return View(user);
    }

    [HttpGet]
    [Route("/User/Delete/{login}")]
    public IActionResult Delete(string login)
    {
        if (!CanAccess(HttpContext))
        {
            return RedirectToAction("AccessDenied"); 
        } 
        
        var user = _userRepository.Get(login);
        if (user is null)
        {
            return RedirectToAction("List");
        }
        
        _userRepository.Delete(user);
        return RedirectToAction("List");
    }

    private static bool CanAccess(HttpContext ctx)
    {
        try
        {
            var userSession =
                JsonSerializer.Deserialize<UserSessionInfomation>(ctx.Session.GetString("session") ?? string.Empty);
            return userSession?.isAdmin ?? false;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}