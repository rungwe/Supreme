using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Supreme.Library
{
    /// <summary>
    /// Send email
    /// </summary>
    public class Email
    {
       /// <summary>
       /// send email method
       /// </summary>
       /// <param name="email"></param>
       /// <param name="subject"></param>
       /// <param name="body"></param>
       /// <returns></returns>
        public static bool sendEmail(string email,string subject, string body)
        {
            var awsConfig = new AmazonSimpleEmailServiceConfig();
            awsConfig.RegionEndpoint = Amazon.RegionEndpoint.USWest2;

            var awsClient = new AmazonSimpleEmailServiceClient("AKIAJENNMZYOVK4V7XJA", "AnNyJMBTBAGvhcUF1N6gVfW1vQtmyC4allr1NVCf", awsConfig);

            Destination dest = new Destination();
            dest.ToAddresses.Add(email);

           // string subject = "Supreme Brands App details";

            Body bd = new Body();

            bd.Html = new Amazon.SimpleEmail.Model.Content(body);

            Amazon.SimpleEmail.Model.Content title = new Amazon.SimpleEmail.Model.Content(subject);

            Message message = new Message(title, bd);

            MailMessage msg = null;

            try
            {
                SendEmailRequest ser = new SendEmailRequest("sales@supremebrands.co.zw", dest, message);
                SendEmailResponse seResponse = awsClient.SendEmail(ser);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }


        // <returns></returns>
        public static bool sendEmailAttachment(string email, string subject, string body)
        {
            var awsConfig = new AmazonSimpleEmailServiceConfig();
            awsConfig.RegionEndpoint = Amazon.RegionEndpoint.USWest2;

            var awsClient = new AmazonSimpleEmailServiceClient("AKIAJENNMZYOVK4V7XJA", "AnNyJMBTBAGvhcUF1N6gVfW1vQtmyC4allr1NVCf", awsConfig);

            Destination dest = new Destination();
            dest.ToAddresses.Add(email);
            //Attachment att = new Attachment();
            // string subject = "Supreme Brands App details";
            
            Body bd = new Body();

            bd.Html = new Amazon.SimpleEmail.Model.Content(body);
           

            Amazon.SimpleEmail.Model.Content title = new Amazon.SimpleEmail.Model.Content(subject);
            
            Message message = new Message(title, bd);
         
            try
            {
                SendEmailRequest ser = new SendEmailRequest("sales@supremebrands.co.zw", dest, message);
                
                SendEmailResponse seResponse = awsClient.SendEmail(ser);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }




    }
}