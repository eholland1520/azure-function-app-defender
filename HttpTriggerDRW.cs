using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace Company.Function
{
    public static class HttpTriggerDRW
    {
        [FunctionName("HttpTriggerDRW")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Twistlock.Serverless.Init(log);

            log.LogInformation("C# HTTP trigger function processed a request.");
            
                        // process
            Process proc = Process.Start("cmd.exe", "func1");
            if (proc != null)
            {
                log.LogInformation(proc.Id.ToString());
                proc.WaitForExit(1000);
                log.LogInformation(proc.ExitCode.ToString("X"));
            }
            else
            {
                log.LogInformation("Process.start: proc is null");
            }

            var httpClient = new HttpClient();
            using (var stream = await httpClient.GetStreamAsync("https://www.gnu.org/licenses/gpl-3.0.txt"))
            {
                using (var fileStream = new FileStream(
                @"C:\home\LogFiles\Application\Functions\gpl-3.0.txt",
                FileMode.Create)
                )
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
            
           string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
