using htdc_api.Models;

namespace htdc_api.Interface;

public interface IEmailSender
{
    void SendEmail(Message message);
}