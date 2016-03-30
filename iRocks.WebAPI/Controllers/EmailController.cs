using iRocks.AI.Helpers.Loging;
using iRocks.DataLayer;
using iRocks.WebAPI.Filters;
using iRocks.WebAPI.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace iRocks.WebAPI.Controllers
{
    public class EmailController : BaseApiController
    {
        public EmailController(IUserRepository userRepository, IPostRepository postRepository, IVoteRepository voteRepository, ICategoryRepository categoryRepository)
            : base(userRepository, postRepository, voteRepository, categoryRepository)
        {
        }
        [LoggingAspect]
        public HttpResponseMessage Post(bool isBugReport, [FromBody]EmailModel emailInfo)
        {
            if (isBugReport)
            {
                if (SendEmail("contact@footprint.social", emailInfo.Body, "contact@footprint.social", "[IMPORTANT] Bug report / Satisfaction form"))
                    return Request.CreateErrorResponse(HttpStatusCode.OK, "mail sent");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Mail cannot be sent");
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Only Bug report can be sent when user is not loged in.");
        }
        [iRocksAuthorizeAttribute]
        [LoggingAspect]
        public HttpResponseMessage Post([FromBody]EmailModel emailInfo)
        {
            try
            {
                return SendInvitation(emailInfo.To, emailInfo.Body);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        public HttpResponseMessage SendInvitation(string to, string message)
        {

            var sender = TheUserRepository.Select(DephtLevel.UserBasic, new { UserName = User.Identity.Name }).SingleOrDefault();
            if (sender != null)
            {
                string from = "contact@footprint.social";

                string template =
@"<html>
    <body>
        Hi,<br/>
        @Model.FirstName @Model.LastName invites you to try this new wonderfull app : http://footprint.social !<br/>
        @Model.Message<br/>
        <strong>See you soon !</strong><br/>
        The Social Footprint team
    </body>
</html>";

                string subject = "Create your own digital Footprint";
                string body = Engine.Razor.RunCompile(template, "invitationTemplate", null, new {FirstName= sender.FirstName, LastName = sender.LastName, Message = message });
                if (SendEmail(to, body, from, subject, true))
                    return Request.CreateErrorResponse(HttpStatusCode.OK, "mail sent");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, "error during sending email...");

            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No user loged in");
        }

        public bool SendEmail(string to, string body, string from, string subject, bool isHtml =false)
        {


            MailMessage message = new MailMessage(from, to);
            message.IsBodyHtml = isHtml;
            message.Subject = subject;
            message.Body = body;

            SmtpClient client = new SmtpClient("mail.gandi.net", 25);
            // Credentials are necessary if the server requires the client  
            // to authenticate before it will send e-mail on the client's behalf.
            client.Credentials = new System.Net.NetworkCredential("contact@footprint.social", "f00tpr1nt*ù");

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }



}
