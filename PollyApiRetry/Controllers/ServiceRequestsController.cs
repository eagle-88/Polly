using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;
using RestSharp;

namespace PollyApiRetry.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ILogger<ServiceRequestsController> _logger;

        public ServiceRequestsController(ILogger<ServiceRequestsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetData")]
        public IActionResult Get()
        {
            //var retryPolly = Policy.Handle<Exception>()
            //                       .Retry(5, onRetry: (exception, retryCount) =>
            //                       {
            //                           Console.WriteLine("Error: " + exception.Message + " ... Retry Count" + retryCount);
            //                       });

            //retryPolly.Execute(() =>
            //{
            //    ConnectToApi();
            //});

            var amountToPause = TimeSpan.FromSeconds(15);

            var retryWaitPolicy = Policy.Handle<Exception>()
                                        .WaitAndRetry(5, i => amountToPause, onRetry: (exception, retryCount) =>
                                        {
                                            Console.WriteLine("Error: " + exception.Message + " ... Retry Count: " + retryCount);
                                        });
            retryWaitPolicy.Execute(() =>
            {
                Console.WriteLine("Executing ...");
                ConnectToApi();
            });

            //var retryPolicy = Policy.Handle<Exception>()
            //                        .WaitAndRetry(5, i => amountToPause, (exception, retryCount) =>
            //                        {
            //                            Console.WriteLine("Error: " + exception.Message + " ... Retry Count: " + retryCount);
            //                        });
            //var circuitBreakerPolicy = Policy.Handle<Exception>()
            //                                 .CircuitBreaker(3, TimeSpan.FromSeconds(30));

            //var finalPolicy = retryPolicy.Wrap(circuitBreakerPolicy);

            //finalPolicy.Execute(() =>
            //{
            //    Console.WriteLine("Executing ...");
            //    ConnectToApi();
            //});
            return Ok();
        }

        private void ConnectToApi()
        {
            var url = "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random";
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept", "application/json");
            request.AddHeader("X-RapidAPI-Key", "5c686c6905mshb5be39f27d175b5p1d22ccjsn4c31efd53d0f");
            request.AddHeader("X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com");
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine(response.Content);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
                throw new Exception("Error ne!!!");
            }
        }
    }
}
