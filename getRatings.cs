using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace ServerlessOH
{
  public static class getRatings
  {
    [FunctionName("getRatings")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
        [Table("ratings", "RATING")] CloudTable ratingsTable,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      string userId = req.Query["userId"];

      if (string.IsNullOrEmpty(userId))
      {
        return new BadRequestObjectResult("Pass the user id");
      }

      var ratings = new List<RatingResponse>();
      string filter = TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId);
      var query = new TableQuery<RatingResponse>().Where(filter);
      TableContinuationToken continuationToken = null;
      

      do
      {
        var page = await ratingsTable.ExecuteQuerySegmentedAsync(query, continuationToken);
        continuationToken = page.ContinuationToken;
        ratings.AddRange(page.Results);
      }
      while (continuationToken != null);

      return new OkObjectResult(ratings);
    }
  }
}
