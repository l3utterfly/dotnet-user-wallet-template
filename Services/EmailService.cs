using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WebApi.Helpers;

namespace WebApi.Services
{
    public class EmailService
    {
        private readonly AppSettings _appsettings;

        public EmailService(
            IOptions<AppSettings> appsettings)
        {
            _appsettings = appsettings.Value;
        }

        public async Task SendEmail(string recipient, string subject, string htmlbody)
        {
            using (var httpclient = new HttpClient())
            {
                var sendgridendpoint = "https://api.sendgrid.com/v3/mail/send";

                // dynamic sendgrid content: https://sendgrid.com/docs/API_Reference/Web_API_v3/Mail/index.html
                dynamic obj = new
                {
                    personalizations = new[]
                    {
                        new
                        {
                            to = new []
                            {
                                new
                                {
                                    email = recipient
                                }
                            },

                            subject = subject
                        }
                    },

                    from = new
                    {
                        email = _appsettings.FromEmailAddress,
                        name = "MMOCircles"
                    },

                    content = new[]
                    {
                        new
                        {
                            type = "text/html",
                            value = htmlbody
                        }
                    }
                };

                var httpRequestMessage = new HttpRequestMessage();

                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(sendgridendpoint);
                httpRequestMessage.Headers.Add("Authorization", "Bearer " + _appsettings.SendGridAPIKey);
                httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

                await httpclient.SendAsync(httpRequestMessage);
            }
        }
    }
}
