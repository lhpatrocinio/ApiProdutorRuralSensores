using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ProdutorRuralSensores.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutorRuralSensoresController : ControllerBase
    {
    }
}
