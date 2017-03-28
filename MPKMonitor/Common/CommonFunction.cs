using System;
using Windows.UI.Popups;
using Windows.ApplicationModel.Email;

namespace MPKMonitor.Common
{
  class CommonFunction
  {
    /// <summary>
    /// Get url with current timestamp
    /// </summary>
    /// <returns>Url</returns>
    public static string GetUrl()
    {
      var ts1 = DateTime.Now - new DateTime(1970, 1, 1);
      string ts = Convert.ToInt64(ts1.TotalMilliseconds).ToString();
      string url = "https://www.peka.poznan.pl/vm/method.vm?ts=" + ts;

      return url;
    }

    /// <summary>
    /// Repleace special character from String
    /// </summary>
    /// <param name="pattern">String to Replace</param>
    /// <returns>String without special character</returns>
    public static string RepleaceSpecialCharacter(string pattern)
    {
      try
      {
        pattern = pattern.Replace("\\", "");
        pattern = pattern.Replace("%", "");
        pattern = pattern.Replace("&", "");
        pattern = pattern.Replace("\"", "");
      }
      catch (Exception ex)
      {
        SendException(ex, "Pattern: " + pattern,"");
      }
      
      return pattern;
    }

    /// <summary>
    /// Show MessageBox on top screen
    /// </summary>
    /// <param name="message">Message to show</param>
    public static async void MessageBoxShow(string message)
    {
      try
      {
        MessageDialog msgbox = new MessageDialog(message);
        await msgbox.ShowAsync();
      }
      catch (Exception ex)
      {
        SendException(ex,"","");
      }
    }

    /// <summary>
    /// SendException to developer
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <param name="AdditionalInfo">Additional infos</param>
    public static async void SendException(Exception ex, string AdditionalInfo, string AdditionalInfo2)
    {
      try
      {
        EmailRecipient sendTo = new EmailRecipient()
        {
          Name = "Michał Stojek",
          Address = "stojexlab@outlook.com"
        };

        // Create email object
        EmailMessage mail = new EmailMessage();
        mail.Subject = "Awaria aplikacji PEKAMonitor";
        mail.Body = ex.ToString();
        mail.Body += "\n==================================================\n";
        mail.Body += AdditionalInfo;
        mail.Body += "\n==================================================\n";
        mail.Body += AdditionalInfo2;

        // Add recipients to the mail object
        mail.To.Add(sendTo);
        //mail.Bcc.Add(sendTo);
        //mail.CC.Add(sendTo);

        // Open the share contract with Mail only:
        await EmailManager.ShowComposeNewEmailAsync(mail);
      }
      catch (Exception)
      {

      }
    }
  }
}
