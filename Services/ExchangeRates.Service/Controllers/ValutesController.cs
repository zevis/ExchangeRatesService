using System;
using System.Threading.Tasks;
using ExchangeRates.Interfaces.BL;
using ExchangeRates.Interfaces.BL.Exceptions;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace ExchangeRates.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValutesController : ControllerBase
    {
        private readonly Logger _logger;
        private readonly IRatesLogic _ratesLogic;

        public ValutesController(Logger logger, IRatesLogic rates_logic)
        {
            _logger = logger;
            _ratesLogic = rates_logic;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime? on_date, [FromQuery] string valute_code)
        {
            try
            {
                return Ok(await _ratesLogic.GetRateOnDateAsync(valute_code, on_date));
            }
            catch (BadRequestException bad_request)
            {
                _logger.Debug(bad_request.Message);
                return BadRequest(bad_request.Message);
            }
            catch (NotFoundException not_found)
            {
                _logger.Debug(not_found.Message);
                return NotFound(not_found.Message);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return StatusCode(500);
            }
        }
    }
}
