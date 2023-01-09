using Microsoft.AspNetCore.Mvc;

namespace ReflectionHelper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IServiceProvider _serviceProvider;
        public WeatherForecastController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public string Get()
        {
            var type = ReflectionHelper.GetTypeByName("ITestClass");
            var instance = _serviceProvider.GetService(type);
            var method = ReflectionHelper.GetMethodInfo("ITestClass", "GetConnectionString");
            return method.Invoke(instance, new object[] {"xxx" }) as string;
        }
    }
}