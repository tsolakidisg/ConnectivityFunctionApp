using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ConnectivityFunctionApp.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace ConnectivityFunctionApp
{
    public static class ReadFromDB
    {
        [FunctionName("ReadFromDB")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Sql("SELECT * FROM [SalesLT].[CustomerAddress] where CustomerID=@id",
            CommandType = System.Data.CommandType.Text,
            Parameters ="@id={Query.id}",
            ConnectionStringSetting = "sqldb_connection")] IEnumerable<CustomerAddress> result,

            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            id = id ?? data?.id;

            string responseMessage = string.IsNullOrEmpty(id)
                ? "This HTTP triggered function executed and updated successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"User with id: {id}, has address type {result.FirstOrDefault().AddressType}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
