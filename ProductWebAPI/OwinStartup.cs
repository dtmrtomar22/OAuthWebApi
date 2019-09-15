using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using ProductWebAPI.Models;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Security;
using System.IO;
using System.Text;

//Follow below post
//https://www.c-sharpcorner.com/uploadfile/ff2f08/token-based-authentication-using-asp-net-web-api-owin-and-i/
// Custom Tables if you want to use
//https://stackoverflow.com/questions/51536735/web-api-owin-how-to-validate-token-on-each-request
//where is the token is stored and how it is validated by OWIN
//https://stackoverflow.com/questions/40532074/where-does-web-api-store-generated-tokens-in-order-to-validate-subsequent-reques/40533204


[assembly: OwinStartup(typeof(ProductWebAPI.Startup))]
namespace ProductWebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<ProductAuthDbContext>(() => new ProductAuthDbContext());
            app.CreatePerOwinContext<UserManager<IdentityUser>>(CreateManager);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            var OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/token"),
                Provider = new AuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                AllowInsecureHttp = true,
            };
            //app.Use(EditResponse); //edit response
            app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }


        public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                //if(context.ClientId!=null)
                    context.Validated();

                return Task.FromResult<object>(null); 
                //string clientId;
                //string clientSecret;

                //if (context.TryGetBasicCredentials(out clientId, out clientSecret))
                //{
                //    // validate the client Id and secret 
                //    context.Validated();
                //}
                //else
                //{
                //    context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                //    context.Rejected();
                //}
            }
            public override Task TokenEndpoint(OAuthTokenEndpointContext context)
            {
                foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
                {
                    context.AdditionalResponseParameters.Add(property.Key, property.Value);
                }
                return Task.FromResult<object>(null);
            }
            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                UserManager<IdentityUser> userManager = context.OwinContext.GetUserManager<UserManager<IdentityUser>>();
                IdentityUser user;
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch(Exception ex)
                {
                    // Could not retrieve the user due to error.
                    context.SetError("server_error");
                    context.Rejected();
                    return;
                }
                if (user != null)
                {
                    //ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                    //                                        user,
                    //                                        DefaultAuthenticationTypes.ExternalBearer);
                    

                    List<Claim> oAuthClaim = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Email, "XXXXXXXX"),
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                    var identity = new ClaimsIdentity(oAuthClaim,context.Options.AuthenticationType);
                    var data = new Dictionary<string, string>
                    {                       
                        
                        { "userName", user.UserName},                              
                        { "isAuthenticated"  , "true"},
                        { "canAccessCategory", "true"},
                        { "canAccessProducts", "true"},
                        { "canAddProducts","true"},  
                        { "canSaveProduct" ,"true"},
                        { "canAddCategory" ,"true"}
                    };
                    var properties = new AuthenticationProperties(data);

                    var ticket = new AuthenticationTicket(identity, properties);

                    //context.Validated(token);                    
                    context.Validated(ticket);
                    
                }
                else
                {
                    context.SetError("invalid_grant", "Invalid UserId or password'");
                    context.Rejected();
                }
            }
        }

        private static UserManager<IdentityUser> CreateManager(IdentityFactoryOptions<UserManager<IdentityUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<IdentityUser>(context.Get<ProductAuthDbContext>());
            var owinManager = new UserManager<IdentityUser>(userStore);
            return owinManager;
        }

      
         public static async Task EditResponse(IOwinContext context, Func<Task> next)
    {
        // get the original body
        var body = context.Response.Body;

        // replace the original body with a memory stream
        var buffer = new MemoryStream();
        context.Response.Body = buffer;

        // invoke the next middleware from the pipeline
        await next.Invoke();

        // get a body as string
        var bodyString = Encoding.UTF8.GetString(buffer.GetBuffer());

        // make some changes to the body
        bodyString = "The body has been replaced.Original body:";

        // update the memory stream
        var bytes = Encoding.UTF8.GetBytes(bodyString);
        buffer.SetLength(0);
        buffer.Write(bytes, 0, bytes.Length);

        // replace the memory stream with updated body
        buffer.Position = 0;
        await buffer.CopyToAsync(body);
        context.Response.Body = body;
    }


    }
}