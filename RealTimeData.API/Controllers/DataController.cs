using Microsoft.AspNetCore.Mvc;
using RealTimeData.API.Models;
using RealTimeData.API.Services;

namespace RealTimeData.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;
        private readonly ILogger<DataController> _logger;

        public DataController(DataService dataService, ILogger<DataController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataPoint>>> GetLatestDataPoints([FromQuery] int count = 100)
        {
            try
            {
                var dataPoints = await _dataService.GetLatestDataPointsAsync(count);
                return Ok(dataPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data points");
                return StatusCode(500, "An error occurred while retrieving data points");
            }
        }

        // Endpoint to simulate data insertion (for testing)
        [HttpPost("generate")]
        public async Task<ActionResult> GenerateRandomData()
        {
            try
            {
                await _dataService.InsertRandomDataAsync();
                return Ok("Random data inserted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating random data");
                return StatusCode(500, "An error occurred while generating random data");
            }
        }
    }
}