using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace Company.Function
{
    public static class FakeACall
    {
        [FunctionName("FakeACall")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string number = req.Query["number"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            number = number ?? data?.number;

            string accountSid = Environment.GetEnvironmentVariable("ACCOUNTSID");
            string authToken = Environment.GetEnvironmentVariable("AUTHTOKEN");

            TwilioClient.Init(accountSid, authToken);

            var to = new PhoneNumber(number);
            var from = new PhoneNumber(Environment.GetEnvironmentVariable("TWILIONUMBER"));

            var call = CallResource.Create(to, from,
            twiml: new Twiml("<Response><Say>Hi there. Thanks for testing the Fake Call function. Hope you enjoy Cloud Summit!</Say></Response>"));

            string responseMessage = call.Sid;

            return new OkObjectResult(responseMessage);
        }
    }
}
