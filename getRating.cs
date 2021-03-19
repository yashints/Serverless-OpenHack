using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;

namespace ServerlessOH
{
    public static class getRating
    {
        [FunctionName("getRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("ratings", "RATING")] CloudTable ratingsTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string ratingId = req.Query["ratingId"];

            if(string.IsNullOrEmpty(ratingId)) {
                return new BadRequestObjectResult("Pass the rating id");
            }

            var query = TableOperation.Retrieve<RatingResponse>("RATING", ratingId);
            var rating = await ratingsTable.ExecuteAsync(query);

            if(rating == null || rating.Result == null) {
                return new NotFoundResult();
            }

            return new OkObjectResult(rating.Result);
        }
    }
}
