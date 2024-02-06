using Microsoft.AspNetCore.Mvc;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.Enums;
using GarageProject.Service;

namespace GarageProject.Controllers;

[ApiController, Route("prepopulate")]
public class PrepopulateController : ControllerBase
{
    private readonly IPrepopulate _prepopulate;
    private readonly ILoggerService _loggerService;

    public PrepopulateController(IPrepopulate prepopulate, ILoggerService loggerService )
    {
        _prepopulate = prepopulate;
        _loggerService = loggerService;
    }

    [HttpGet]
    public async Task<IActionResult> Prepopulate()
    {
        try
        {
            //await _prepopulate.PrepopulateInMemory();
            await _prepopulate.PrepopulateDB();
            string message = "DB has been prepopulated";
            _loggerService.Log(message);
            return Ok(message);
        }
        catch (Exception e)
        {
            _loggerService.Log( e.Message );
            return BadRequest("something went wrong");
        }
        
    }

    [HttpDelete]
    public async Task<IActionResult> ClearDb()
    {
        try
        {
            await _prepopulate.ClearDb();
            string message = "DB has been cleared";
            _loggerService.Log( message);
            return Ok(message);
        }
        catch (Exception e)
        {
            _loggerService.Log( e.Message );
            return BadRequest("something went wrong");
        }
    }
}