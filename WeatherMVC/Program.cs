using WeatherMVC.Configurations;
using WeatherMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.Configure<IdentityServerSettings>(builder.Configuration.GetSection(nameof(IdentityServerSettings)));

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = "cookie";
    opt.DefaultChallengeScheme = "oidc";
})
    .AddCookie("cookie", opt =>
    {
        opt.Cookie.Name = "auth";

        var authCookieExpiration = TimeSpan.FromSeconds(builder.Configuration.GetSection("AuthCookie:ExpirationSeconds").Get<int>());
        opt.ExpireTimeSpan = authCookieExpiration;
        opt.Cookie.MaxAge = authCookieExpiration;

        opt.Cookie.SameSite = SameSiteMode.Strict;
    })
    .AddOpenIdConnect("oidc", opt =>
    {
        opt.Authority = builder.Configuration["InteractiveAuthServiceSettings:AuthorityUrl"];
        opt.ClientId = builder.Configuration["InteractiveAuthServiceSettings:ClientId"];
        opt.ClientSecret = builder.Configuration["InteractiveAuthServiceSettings:ClientSecret"];

        var scopes = builder.Configuration.GetSection("InteractiveAuthServiceSettings:Scopes").Get<List<string>>();
        foreach (var scope in scopes)
            opt.Scope.Add(scope);

        opt.ResponseType = "code";
        opt.UsePkce = true;
        opt.ResponseMode = "query";
        opt.SaveTokens = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
