namespace MyFirstApi.Services
{
    public class GreetingService : IGreetingService
    {
        public string CreateGreating(string name)
        {
            return $"Hej fra service, {name}!";
        }
    }
}
