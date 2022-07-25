using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ModelCompiler;
using System.Linq;

namespace ua.model2nodeset.function
{
    public static class GenerateNodesetXML
    {
        [FunctionName("GenerateNodesetXML")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string xml = data?.xml;

            var dir = Path.GetTempPath();
            string sessionId = Guid.NewGuid().ToString().Split("-")[0];
            var dirInfo = Directory.CreateDirectory($"{dir}{sessionId}");
            log.LogInformation(dirInfo.FullName);

            var xmlFileUrl = $@"{dirInfo.FullName}\input.xml";
            File.WriteAllText(xmlFileUrl, xml);
            log.LogInformation("xml created");

            try
            {
                var args = new string[]
                    {
        "compile",
        "-d2",
        $@"{dirInfo.FullName}\input.xml",
        "-cg",
        $@"{dirInfo.FullName}\input.csv",
        "-o2",
        $"{dirInfo.FullName}"
                    };

                for (int ii = 0; ii < args.Length; ii++)
                {
                    args[ii] = args[ii].Replace("\n", "\\n");
                }

                ModelCompilerApplication.Run(args);

            }
            catch (Exception e)
            {
                log.LogInformation("error message:");
                log.LogInformation(e.Message);
            }

            // find the nodeset2.xml file
            string xmlNodeSet = File.ReadAllText(Directory.EnumerateFiles($"{dirInfo.FullName}").First(x => x.Contains("NodeSet2.xml")));

            // clean up
            Directory.Delete(dirInfo.FullName, true);

            return new OkObjectResult(xmlNodeSet);
        }
    }
}
