using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RAI_02.Models;
using RAI_02.Repositories;

namespace RAI_02.Controllers;

public class FriendsController : Controller
{
    private readonly IUserRepository _userRepository;

    public FriendsController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public IActionResult Index()
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }

        var user = _userRepository.Get(userSession.Login);
        if (user is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        return View(user.Friends.Values);
    }

    [HttpGet]
    public IActionResult Add()
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        return View();
    }

    [HttpPost]
    public IActionResult Add(string login)
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        var user = _userRepository.Get(userSession.Login);
        if (user is null)
        {
            return RedirectToAction("Index", "Login");
        }

        var newFriend = _userRepository.Get(login);
        return Json(newFriend is not null && user.AddFriend(newFriend));
    }

    [HttpGet]
    public IActionResult Delete(string login)
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        var user = _userRepository.Get(userSession.Login);
        if (user is null)
        {
            return RedirectToAction("Index", "Login");
        }

        var friendToRemove = _userRepository.Get(login);
        return Json(friendToRemove is not null && user.RemoveFriend(friendToRemove));
    }

    [HttpGet]
    public IActionResult List()
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        var user = _userRepository.Get(userSession.Login);
        if (user is null)
        {
            return RedirectToAction("Index", "Login");
        }

        return Json(user.Friends);
    }

    [HttpGet]
    public async Task<IActionResult> Export()
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        var user = _userRepository.Get(userSession.Login);
        if (user is null)
        {
            return RedirectToAction("Index", "Login");
        }

        var options = new JsonSerializerOptions();
        options.Converters.Add(new UserConverter());
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, user.Friends.Values, options);
        await ms.FlushAsync();
        ms.Position = 0;
        

        return File(ms, "application/json", "friends.json");
    }

    [HttpPost]
    public async Task<IActionResult> Import()
    {
        var userSession = HttpContext.Session.GetSessionInfomation();
        if (userSession is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        var user = _userRepository.Get(userSession.Login);
        if (user is null)
        {
            return RedirectToAction("Index", "Login");
        }
        
        var stream = Request.Form.Files["file"]!.OpenReadStream();
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNameCaseInsensitive = false
        };
        options.Converters.Add(new UserConverter());
        var users = (await JsonSerializer.DeserializeAsync<IEnumerable<User>>(stream, options) ?? Array.Empty<User>()).ToArray();
        
        if (!users.Any())
        {
            ViewData["fileBadFormat"] = true;
            return RedirectToAction("Index");
        }
        
        var validationResult = _userRepository.Validate(users);
        ViewData["validationResult"] = validationResult;
        ViewData["importResult"] = user.AddFriendRange(validationResult.validUsers);
        return RedirectToAction("Index");
    }
}