using System;
using FluentValidation;
using Microsoft.WindowsAzure.Storage.Table;

namespace ServerlessOH
{
  public class RatingInput : TableEntity
  {
    public string UserId { get; set; }
    public string ProductId { get; set; }
    public string LocationName { get; set; }
    public int Rating { get; set; }
    public string UserNotes { get; set; }
  }

  public class RatingResponse : RatingInput
  {
    public string Id { get; set; }
    public RatingResponse(RatingInput input)
    {
      UserId = input.UserId;
      ProductId = input.ProductId;
      LocationName = input.LocationName;
      Rating = input.Rating;
      UserNotes = input.UserNotes;
      Id = Guid.NewGuid().ToString();
      Timestamp = DateTime.UtcNow;
      PartitionKey = "RATING";
      RowKey = Id;
    }

    public RatingResponse()
    {
        
    }
  }

  public class RatingInputValidator : AbstractValidator<RatingInput>
  {
    public RatingInputValidator()
    {
      RuleFor(x => x.UserId).NotEmpty();
      RuleFor(x => x.ProductId).NotEmpty();
      RuleFor(x => x.Rating).NotEmpty()
        .GreaterThanOrEqualTo(0)
        .LessThanOrEqualTo(5);
    }
  }
}