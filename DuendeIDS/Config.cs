using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;
using System.Security.Claims;
using System.Text.Json;

namespace Duende
{
    public class Config
    {
        public static List<TestUser> TestUsers
        {
            get
            {
                var aliceAddress = new
                {
                    street_address = "Sarmiento 789",
                    locality = "Villa Ramallo",
                    postal_code = 2000,
                    country = "Argentina"
                };

                var aliceUser = new TestUser
                {
                    SubjectId = "346858",
                    Username = "alice",
                    Password = "alicePassword",
                    Claims =
                    {
                      new Claim(JwtClaimTypes.Name, "Alice Smith"),
                      new Claim(JwtClaimTypes.GivenName, "Alice"),
                      new Claim(JwtClaimTypes.FamilyName, "Smith"),
                      new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "admin"),
                      new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                      new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(aliceAddress), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                };

                var bobAddress = new
                {
                    street_address = "Moreno 123",
                    locality = "San Pedro",
                    postal_code = 2100,
                    country = "Argentina"
                };

                var bobUser = new TestUser
                {
                    SubjectId = "88421113",
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                      new Claim(JwtClaimTypes.Name, "Bob Smith"),
                      new Claim(JwtClaimTypes.GivenName, "Bob"),
                      new Claim(JwtClaimTypes.FamilyName, "Smith"),
                      new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "user"),
                      new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                      new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(bobAddress), IdentityServerConstants.ClaimValueTypes.Json)
                    }
                };

                return new List<TestUser>
                {
                    aliceUser,
                    bobUser
                };
            }
        }

        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                  Name = "role",
                  UserClaims = new List<string> {"role"}
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("weatherapi.read"),
                new ApiScope("weatherapi.write"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("weatherapi")
                {
                    Scopes = new List<string> { "weatherapi.read", "weatherapi.write" },
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256()) },
                    UserClaims = new List<string> { "role" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "weather-mvc",
                    ClientName = "Weather MVC Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("SuperSecretPassword".Sha256()) },

                    AllowedScopes = { "weatherapi.read", "weatherapi.write"}
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:5444/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:5444/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "weatherapi.read" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                }
            };
    }
}
