using System.Net;
using Microsoft.AspNetCore.Mvc;
using Certes.Jws;
using Certes;
using static Certes.Helper;

var host = WebApplication.CreateSlimBuilder(args);

var app = host.Build();

app.MapGet(".well-known/acme-challenge/{*token}", async (HttpContext context, [FromRoute] string token) =>
{
    var accountKey = GetTestKey(context.Request.Host.Host);

    context.Response.StatusCode = (int)HttpStatusCode.OK;
    await context.Response.WriteAsync(accountKey.KeyAuthorization(token));
});

app.MapPut("dns-01/{algo}", async (HttpContext req, [FromRoute] string algo, [FromBody] Dictionary<string, string> tokens) =>
{
    throw new NotImplementedException();
    // var addedRecords = new Dictionary<string, string>();
    // var keyType = (KeyAlgorithm)Enum.Parse(typeof(KeyAlgorithm), algo, true);
    // var accountKey = GetTestKey(keyType);
    // 
    // var loginInfo = new ServicePrincipalLoginInformation
    // {
    //     ClientId = Env("CERTES_AZURE_CLIENT_ID"),
    //     ClientSecret = Env("CERTES_AZURE_CLIENT_SECRET"),
    // };
    // 
    // var credentials = new AzureCredentials(loginInfo, Env("CERTES_AZURE_TENANT_ID"), AzureEnvironment.AzureGlobalCloud);
    // var builder = RestClient.Configure();
    // var resClient = builder.WithEnvironment(AzureEnvironment.AzureGlobalCloud)
    //     .WithCredentials(credentials)
    //     .Build();
    // using (var client = new DnsManagementClient(resClient))
    // {
    //     client.SubscriptionId = Env("CERTES_AZURE_SUBSCRIPTION_ID");
    // 
    //     foreach (var p in tokens)
    //     {
    //         var name = "_acme-challenge." + p.Key.Replace(".dymetis.com", "");
    //         var value = accountKey.SignatureKey.DnsTxt(p.Value);
    //         await client.RecordSets.CreateOrUpdateAsync(
    //             "dymetis",
    //             "dymetis.com",
    //             name,
    //             RecordType.TXT,
    //             new RecordSetInner(
    //                 name: name,
    //                 tTL: 1,
    //                 txtRecords: new[] { new TxtRecord(new[] { value }) }));
    // 
    //         addedRecords.Add(name, value);
    //     }
    // }
    // 
    // var response = req.CreateResponse(HttpStatusCode.OK);
    // await response.WriteAsJsonAsync(addedRecords);
    // return response;
});

await app.RunAsync();
