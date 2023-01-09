namespace ReflectionHelper
{
    public interface ITestClass
    {
        string GetConnectionString(string connectionString);
    }
    public class TestClass : ITestClass
    {
        private readonly IConfiguration _config;
        public TestClass(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GetConnectionString(string connectionString)
        {
            return _config.GetConnectionString(connectionString);
        }
    }
}
