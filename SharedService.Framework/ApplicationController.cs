using Microsoft.AspNetCore.Mvc;
using SharedService.SharedKernel;

namespace SharedService.Framework;

[ApiController]
[Route("[controller]")]

public abstract class ApplicationController : ControllerBase
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);
        return new(envelope);
    }
}
