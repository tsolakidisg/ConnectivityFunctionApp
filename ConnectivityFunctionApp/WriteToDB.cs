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

namespace ConnectivityFunctionApp
{
    public static class WriteToDB
    {
        [FunctionName("WriteToDB")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "addNewCustomer")] HttpRequest req,
            [Sql("[dbo].[TestTable]", ConnectionStringSetting = "sqldb_connection")] IAsyncCollector<TestTable> testTableItems,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger with SQL Output Binding function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TestTable testTableItem = JsonConvert.DeserializeObject<TestTable>(requestBody);

            await testTableItems.AddAsync(testTableItem);
            await testTableItems.FlushAsync();
            List<TestTable> resultList = new List<TestTable> { testTableItem };

            return new OkObjectResult(resultList);
        }
    }
}
