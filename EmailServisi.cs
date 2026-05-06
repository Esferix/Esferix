using System;
using System.IO;
using System.Net;
using System.Net.Mail;

public class EmailServisi
{
    public static bool EmailGonder(string aliciEmail, string kod)
    {
        try
        {
            var fromAddress = new MailAddress("destek@esferix.com", "Esferix Kargo");
            var toAddress = new MailAddress(aliciEmail);
            string subject = "Şifre Sıfırlama Kodu";
            string body = $"Şifrenizi sıfırlamak için onay kodunuz: {kod}";
            string mailKlasoru = @"C:\EsferixKargo\KargoMailleri\KullanıcıOnayKodu\";

            if (!Directory.Exists(mailKlasoru))
            {
                Directory.CreateDirectory(mailKlasoru);
            }

            var smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = mailKlasoru
            };

            using (var message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;
                message.BodyTransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                smtp.Send(message);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static bool EmailGonder1(string aliciEmail, string kod)
    {
        try
        {
            var fromAddress = new MailAddress("destek @esferix.com", "Esferix Kargo");
            var toAddress = new MailAddress(aliciEmail);
            string subject = "Şifre Sıfırlama Kodu";
            string body = $"Şifrenizi sıfırlamak için onay kodunuz: {kod}";
            string mailKlasoru = @"C:\EsferixKargo\KargoMailleri\PersonelOnayKodu\";

            if (!Directory.Exists(mailKlasoru))
            {
                Directory.CreateDirectory(mailKlasoru);
            }

            var smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = mailKlasoru
            };

            using (var message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;
                message.BodyTransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                smtp.Send(message);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}