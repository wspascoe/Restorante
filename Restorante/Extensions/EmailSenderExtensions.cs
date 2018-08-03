using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Restorante.Services;
using Restorante.Utility;

namespace Restorante.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static Task SendOrderStatusAsync(this IEmailSender emailSender, string email, string orderNumber, string status)
        {
            string subject = "";
            string message = "";

            if(status == SD.StatusCancelled)
            {
                subject = "Order Cancelled";
                message = "Order Number: " + orderNumber + " has been cancelled";
            }

            if (status == SD.StatusSubmitted)
            {
                subject = "Order Created Successfully";
                message = "Order Number: " + orderNumber + " has been submitted";
            }

            if (status == SD.StatusReady)
            {
                subject = "Order Is Ready For Pickup";
                message = "Order Number: " + orderNumber + " is ready for pickup";
            }

            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}
