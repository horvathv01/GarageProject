using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.Enums;

namespace GarageProject.Service;

public class Prepopulate : IPrepopulate
{

    private readonly IRepository<User> _userRepository;
    private readonly IAccessUtilities _hasher;

    private readonly IUserService _userService;

    private readonly GarageProjectContext _context;


    public Prepopulate(
        IRepository<User> userRepository,
        IAccessUtilities hasher,
        IUserService userService,
        GarageProjectContext context
        )
    {
        _userRepository = userRepository;
        _hasher = hasher;
        _userService = userService;
        _context = context;
    }

    public async Task PrepopulateInMemory()
    {

    }

    public async Task PrepopulateDB()
    {

    }

    private async Task AddAssociatedDB()
    {

    }

    private async Task AddNotAssociatedDB()
    {

    }

    public async Task ClearDb()
    {

    }
}