using Duende;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddIdentityServer(opt =>
{
    opt.Events.RaiseFailureEvents = true;
    opt.Events.RaiseErrorEvents = true;
    opt.Events.RaiseSuccessEvents = true;
    opt.Events.RaiseInformationEvents = true;

    opt.EmitStaticAudienceClaim = true;
}).AddTestUsers(Config.TestUsers)
  .AddInMemoryClients(Config.Clients)
  .AddInMemoryApiResources(Config.ApiResources)
  .AddInMemoryApiScopes(Config.ApiScopes)
  .AddInMemoryIdentityResources(Config.IdentityResources);

var app = builder.Build();

app.UseIdentityServer();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();

app.Run();
