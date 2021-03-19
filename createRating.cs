using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net.Http;

namespace ServerlessOH
{
  public static class createRating
  {
    [FunctionName("createRating")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [Table("ratings")] CloudTable ratingTable,
        ILogger log)
    {
      string userIdAPIURL = Environment.GetEnvironmentVariable("UserAPIURL");
      string productAPIURL = Environment.GetEnvironmentVariable("ProductAPIURL");

      var validator = new RatingInputValidator();
      log.LogInformation("C# HTTP trigger function processed a request.");
      var json = await req.ReadAsStringAsync();
      var rating = JsonConvert.DeserializeObject<RatingInput>(json);
      var validationResult = validator.Validate(rating);

      if (!validationResult.IsValid)
      {
        return new BadRequestObjectResult(validationResult.Errors.Select(e => new
        {
          Field = e.PropertyName,
          Error = e.ErrorMessage
        }));
      }

      // Validate user
      if (!(await EntityExists(userIdAPIURL, rating.UserId)))
      {
        return new BadRequestObjectResult("User does not exist!");
      }

      if (!(await EntityExists(productAPIURL, rating.ProductId)))
      {
        return new BadRequestObjectResult("User does not exist!");
      }

      var response = await WriteToTable(ratingTable, rating);

      return new OkObjectResult(response);
    }

    private static async Task<Boolean> EntityExists(string url, string id)
    {
      HttpClient newClient = new HttpClient();
      HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, string.Format(url, id));
      HttpResponseMessage response = await newClient.SendAsync(newRequest);

      return response.StatusCode != System.Net.HttpStatusCode.BadRequest;
    }

    private static async Task<RatingResponse> WriteToTable(CloudTable outputTable, RatingInput input)
    {
      var response = new RatingResponse(input);
      var addEntryOperation = TableOperation.Insert(response);
      await outputTable.ExecuteAsync(addEntryOperation);
      return response;
    }
  }
}

