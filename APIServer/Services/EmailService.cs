using Amazon;
using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using Utility;

namespace Services
{
    public class EmailService
    {
        readonly ILogger logger;
        readonly IAmazonSimpleEmailServiceV2 sesClient;

        string fromAddress => "nexus@im-booth.com";
        string confirmHtml;
        string forgetHtml;
        string unitInfoHtml;

        public EmailService(ILogger<EmailService> logger, IWebHostEnvironment environment)
        {
            this.logger = logger;
            var endpoint = RegionEndpoint.GetBySystemName(GlobalVar.SESEndpointString);
            sesClient = new AmazonSimpleEmailServiceV2Client(GlobalVar.SESAccessKey, GlobalVar.SESSecretKey, endpoint);

            //logger.LogInformation(environment.WebRootPath);
            var confirmPath = Path.Combine(environment.WebRootPath, "confirm.html");
            confirmHtml = File.ReadAllText(confirmPath);

            var forgetPath = Path.Combine(environment.WebRootPath, "forget.html");
            forgetHtml = File.ReadAllText(forgetPath);

            var unitInfoPath = Path.Combine(environment.WebRootPath, "unit-info.html");
            unitInfoHtml = File.ReadAllText(unitInfoPath);
        }

        public async Task<string> SendEmail(string toEmailAddresses, string? subject, string? htmlContent)
        {
            return await SendEmailInternal(new List<string> { toEmailAddresses }, subject, htmlContent);
        }

        async Task<string> SendEmailInternal(List<string> toEmailAddresses, string? subject,
       string? htmlContent, string? textContent = null, string? templateName = null, string? templateData = null, string? contactListName = null)
        {
            var request = new SendEmailRequest
            {
                FromEmailAddress = fromAddress
            };

            if (toEmailAddresses.Any())
            {
                request.Destination = new Destination { ToAddresses = toEmailAddresses };
            }

            if (!string.IsNullOrEmpty(templateName))
            {
                request.Content = new EmailContent()
                {
                    Template = new Template
                    {
                        TemplateName = templateName,
                        TemplateData = templateData
                    }
                };
            }
            else
            {
                request.Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content { Data = subject },
                        Body = new Body
                        {
                            Html = new Content { Data = htmlContent },
                            //Text = new Content { Data = htmlContent }
                        }
                    }
                };
            }

            if (!string.IsNullOrEmpty(contactListName))
            {
                request.ListManagementOptions = new ListManagementOptions
                {
                    ContactListName = contactListName
                };
            }

            try
            {
                var response = await sesClient.SendEmailAsync(request);
                return response.MessageId;
            }
            catch (AccountSuspendedException ex)
            {
                logger.LogInformation("The account's ability to send email has been permanently restricted.");
                logger.LogInformation(ex.Message);
            }
            catch (MailFromDomainNotVerifiedException ex)
            {
                logger.LogInformation("The sending domain is not verified.");
                logger.LogInformation(ex.Message);
            }
            catch (MessageRejectedException ex)
            {
                logger.LogInformation("The message content is invalid.");
                logger.LogInformation(ex.Message);
            }
            catch (SendingPausedException ex)
            {
                logger.LogInformation("The account's ability to send email is currently paused.");
                logger.LogInformation(ex.Message);
            }
            catch (TooManyRequestsException ex)
            {
                logger.LogInformation("Too many requests were made. Please try again later.");
                logger.LogInformation(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"An error occurred while sending the email: {ex.Message}");
            }

            return string.Empty;
        }

        public async Task SendConfirmEmail(string email, string url)
        {
            logger.LogInformation("SendConfirmEmail");
            string[] words = email.Split('@');
            var content = confirmHtml.Replace("{USER_NAME}", words[0]);
            content = content.Replace("{CONFIRM_LINK}", url);
            content = content.Replace("{SERVICE_NAME}", GlobalVar.ServiceName);

            await SendEmail(email, GlobalVar.ServiceName + " Email Confirmation", content);
        }

        public async Task SendForgetPasswordEmail(string email, string url)
        {
            logger.LogInformation("SendForgetPasswordEmail");

            string[] words = email.Split('@');
            var content = forgetHtml.Replace("{USER_NAME}", words[0]);
            content = content.Replace("{RESET_LINK}", url);
            content = content.Replace("{SERVICE_NAME}", GlobalVar.ServiceName);

            await SendEmail(email, GlobalVar.ServiceName + " Password Reset", content);
        }

        public async Task SendUnitInfoEmail(string email, string estateName, string? estateAddress, 
            string unitName, string unitPrice, string? unitImage, string pageUrl,
            string? landArea, string? totalLiving, string? groundFloor, string? firstFloor,
            string? outdoorArea, string? garageArea, string features, string? description, string status)
        {
            logger.LogInformation("SendUnitInfoEmail");

            string[] words = email.Split('@');
            var content = unitInfoHtml.Replace("{USER_NAME}", words[0]);
            content = content.Replace("{ESTATE_NAME}", estateName);
            content = content.Replace("{ESTATE_ADDRESS}", estateAddress ?? "");
            content = content.Replace("{UNIT_NAME}", unitName);
            content = content.Replace("{UNIT_PRICE}", unitPrice);
            content = content.Replace("{PAGE_URL}", pageUrl);

            // Handle image
            var imageHtml = !string.IsNullOrEmpty(unitImage) 
                ? $"<img src=\"{unitImage}\" alt=\"{unitName}\" class=\"unit-image\">" 
                : "";
            content = content.Replace("{UNIT_IMAGE}", imageHtml);

            // Handle areas
            content = content.Replace("{LAND_AREA}", !string.IsNullOrEmpty(landArea) 
                ? $"<div class=\"detail-item\"><div class=\"detail-label\">Land Area</div><div class=\"detail-value\">{landArea} m²</div></div>" : "");
            content = content.Replace("{TOTAL_LIVING}", !string.IsNullOrEmpty(totalLiving) 
                ? $"<div class=\"detail-item\"><div class=\"detail-label\">Total Living</div><div class=\"detail-value\">{totalLiving} m²</div></div>" : "");
            content = content.Replace("{GROUND_FLOOR}", !string.IsNullOrEmpty(groundFloor) 
                ? $"<div class=\"detail-item\"><div class=\"detail-label\">Ground Floor</div><div class=\"detail-value\">{groundFloor} m²</div></div>" : "");
            content = content.Replace("{FIRST_FLOOR}", !string.IsNullOrEmpty(firstFloor) 
                ? $"<div class=\"detail-item\"><div class=\"detail-label\">First Floor</div><div class=\"detail-value\">{firstFloor} m²</div></div>" : "");
            content = content.Replace("{OUTDOOR_AREA}", !string.IsNullOrEmpty(outdoorArea) 
                ? $"<div class=\"detail-item\"><div class=\"detail-label\">Outdoor</div><div class=\"detail-value\">{outdoorArea} m²</div></div>" : "");
            content = content.Replace("{GARAGE_AREA}", !string.IsNullOrEmpty(garageArea) 
                ? $"<div class=\"detail-item\"><div class=\"detail-label\">Garage</div><div class=\"detail-value\">{garageArea} m²</div></div>" : "");

            content = content.Replace("{FEATURES}", features);

            // Handle description
            var descHtml = !string.IsNullOrEmpty(description) 
                ? $"<div class=\"description\">{description}</div>" 
                : "";
            content = content.Replace("{DESCRIPTION}", descHtml);

            // Handle status
            var statusClass = status.ToLower() switch
            {
                "available" => "status-available",
                "reserved" => "status-reserved",
                "sold" => "status-sold",
                _ => "status-available"
            };
            content = content.Replace("{UNIT_STATUS}", $"<span class=\"status-badge {statusClass}\">{status}</span>");

            await SendEmail(email, $"{estateName} - {unitName}", content);
        }
    }
}
