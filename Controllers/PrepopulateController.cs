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

    public PrepopulateController(IPrepopulate prepopulate)
    {
        _prepopulate = prepopulate;
    }

    [HttpGet]
    public async Task<IActionResult> Prepopulate()
    {
        try
        {
            //await _prepopulate.PrepopulateInMemory();
            await _prepopulate.PrepopulateDB();
            string message = "DB has been prepopulated";
            Console.WriteLine(message);
            return Ok(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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
            Console.WriteLine(message);
            return Ok(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("something went wrong");
        }
    }
}