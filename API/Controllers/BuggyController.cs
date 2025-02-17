using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    // 404
    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }

    // 400
    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        return BadRequest("This is not a good request");
    }

    // 401
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        return Unauthorized();
    }

    // 400
    [HttpGet("validation-error")]
    public IActionResult GetValidationError()
    {
        ModelState.AddModelError("Problem1", "This is the first error");
        ModelState.AddModelError("Problem2", "This is the second error");
        return ValidationProblem();
    }

    // 500
    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("This is a server error");
    }
}
