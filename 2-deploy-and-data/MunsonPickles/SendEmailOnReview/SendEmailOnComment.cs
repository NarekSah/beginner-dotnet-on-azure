using Azure.Messaging.EventGrid;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using SendGrid.Helpers.Mail;

namespace SendEmailOnReview
{
    public class SendEmailOnComment
    {
        [Function("SendEmailOnComment")]
        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            var data = eventGridEvent.Data.ToObjectFromJson<CommentData>();

            var emailText = $"New comment on product {data?.ProductId}:\n\n{data?.CommentText}";

            var client = new SendGrid.SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));
            var msg = new SendGrid.Helpers.Mail.SendGridMessage()
            {
                From = new EmailAddress("noreply@yourapp.com"),
                Subject = "New Product Comment",
                PlainTextContent = emailText
            };
            msg.AddTo(new EmailAddress("sahakian.narek@gmail.com"));

            var response = await client.SendEmailAsync(msg);
            log.LogInformation($"Email sent with status {response.StatusCode}");
        }
    }

}
