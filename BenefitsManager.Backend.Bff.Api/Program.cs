using BenefitsManager.Common.Models;
using BenefitsManager.Backend.Bff.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights.Extensibility;
using BenefitsManager.Backend.Bff.Api;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Dapr client
builder.Services.AddDaprClient();

// Add services to the container.
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.Configure<TelemetryConfiguration>((o) => {
    o.TelemetryInitializers.Add(new AppInsightsTelemetryInitializer());
});


builder.Services.AddSingleton<IClaimsManager, ClaimsStoreManager>();
//builder.Services.AddSingleton<IClaimsManager, FakeClaimsManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/claims", async ([FromQuery(Name = "userId")] string userId, IClaimsManager claimsManager) =>
    TypedResults.Ok(await claimsManager.GetClaimsByCreatorAsync(userId)))
    .WithName("GetClaimsByCreator")
    .Produces<List<ClaimModel>>();

app.MapGet("/api/claims/{claimId}", async ([FromRoute] Guid claimId, IClaimsManager claimsManager) =>
   await claimsManager.GetClaimByIdAsync(claimId)
       is ClaimModel claim ? Results.Ok(claim) : Results.NotFound())
           .WithName("GetClaimById")
           .Produces<ClaimModel>()
           .Produces<NotFound>();

app.MapPost("/api/claims", async ([FromBody] ClaimAddModel claimModel, IClaimsManager claimsManager) =>
{
    var claimId = await claimsManager.CreateNewClaimAsync(claimModel.Merchant,
                claimModel.ClaimedAmount,
                claimModel.PurchaseDate,
                claimModel.CategoryCode,
                claimModel.Description,
                claimModel.ReceiptPath,
                claimModel.CreatedBy);

    return Results.Created($"/api/claims/{claimId}", claimId);
}).WithName("CreateNewClaim")
.Produces<Created<Guid>>();

app.MapPut("/api/claims/{claimId}", async ([FromRoute] Guid claimId, [FromBody] ClaimUpdateModel claimModel, IClaimsManager claimsManager) =>
    await claimsManager.UpdateClaimAsync(claimId,
    claimModel.Merchant,
    claimModel.ClaimedAmount,
    claimModel.PurchaseDate,
    claimModel.CategoryCode,
    claimModel.Description,
    claimModel.ReceiptPath)
    is bool updateResult ? Results.Ok(updateResult) : Results.NotFound()
   ).WithName("UpdateClaim")
    .Produces<Ok<bool>>()
    .Produces<NotFound>();

app.MapPut("/api/claims/{claimId}/status", async ([FromRoute] Guid claimId, [FromBody] ClaimStatusUpdateModel claimStatusUpdateModel, IClaimsManager claimsManager) =>
    await claimsManager.UpdateClaimStatusAsync(claimId,
    claimStatusUpdateModel.ApprovedAmount,
    claimStatusUpdateModel.NewStatus,
    claimStatusUpdateModel.Comment,
    claimStatusUpdateModel.SetBy)
    is bool updateResult ? Results.Ok(updateResult) : Results.NotFound()
   ).WithName("UpdateClaimStatus")
    .Produces<Ok<bool>>()
    .Produces<NotFound>();

app.MapDelete("/api/claims/{claimId}", async ([FromRoute] Guid claimId, IClaimsManager claimsManager) =>
    await claimsManager.DeleteClaimAsync(claimId)
    ? Results.Ok() : Results.NotFound()
    ).WithName("DeleteClaim")
    .Produces<Ok>()
    .Produces<NotFound>();

app.Run();
