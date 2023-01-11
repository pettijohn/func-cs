using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace func_cs_isolated
{
    public class IsolatedExample
    {
        private readonly ILogger _logger;

        public IsolatedExample(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<IsolatedExample>();
        }

        [Function("IsolatedExample")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            string responseMessage = shared_logic.ClassThatDoesSomeWork.DoTheWork();
            response.WriteString(responseMessage);

            return response;
        }
    }
}
