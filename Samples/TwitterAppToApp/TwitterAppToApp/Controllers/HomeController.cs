using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace TwitterAppToApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var appCreds = new ConsumerCredentials("xxxxxxxxxxxxxxxxxxxxxxx", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            // Specify the url you want the user to be redirected to
            var redirectURL = "http://" + Request.Url.Authority + "/Home/ValidateTwitterAuth";
            var url = CredentialsCreator.GetAuthorizationURL(appCreds, redirectURL);

            return new RedirectResult(url);
        }

        public ActionResult ValidateTwitterAuth()
        {
            // Get some information back from the URL
            var verifierCode = Request.Params.Get("oauth_verifier");
            var authorizationId = Request.Params.Get("authorization_id");

            // Create the user credentials
            var userCreds = CredentialsCreator.GetCredentialsFromVerifierCode(verifierCode, authorizationId);

            // Do whatever you want with the user now!
            var user = Tweetinvi.User.GetLoggedUser(userCreds);
            user.PublishTweet("BandOntheRun Test");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}