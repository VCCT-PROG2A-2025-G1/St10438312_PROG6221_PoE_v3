using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10438312_PROG6221_PoE_v3
{
    internal class CybersecurityQuiz
    {
        static void cybersecurityQuiz()
        {
            string[] questions =
            {
            "What should you do if you receive an email asking for your password?",
            "What is a strong password?",
            "How can you tell if a website is secure before entering personal information?",
            "What should you do if you suspect a website is a phishing site?",
            "Why is it important to use different passwords for each account?",
            "Which of the following is a sign of a scam message?",
            "What is the best way to protect your privacy online?",
            "Why should you avoid clicking on unknown links in emails?",
            "What is two-factor authentication (2FA)?",
            "When browsing online, what is a safe practice?"
        };

            string[] anwsers =
            {
            "A.) Reply with your password \nB.) Delete the email \nC.) Report the email as phishing \nD.) Ignore it",                                               // C
            "A.) A mix of letters, numbers, and symbols \nB.) 123456 \nC.) Your name and birthdate \nD.) Your pet’s name",                                          // A
            "A.) It looks colorful \nB.) It has ads \nC.) The URL starts with 'https://' and shows a padlock \nD.) It loads quickly",                               // C
            "A.) Close the site and report it \nB.) Bookmark the site \nC.) Enter your details anyway \nD.) Refresh the page",                                      // A
            "A.) It saves time \nB.) It’s easier to remember \nC.) So your friends can guess them \nD.) If one gets stolen, the others remain safe",                // D
            "A.) Poor spelling and grammar \nB.) Urgent tone asking for money \nC.) Suspicious links \nD.) All of the above",                                       // D
            "A.) Share your login on social media \nB.) Limit what personal info you share \nC.) Use incognito mode always \nD.) Accept all cookies",               // B
            "A.) They open cool games \nB.) They might install malware or steal data \nC.) They update your browser \nD.) They show interesting videos",            // B
            "A.) An extra layer of login security like a code sent to your phone \nB.) A method using two websites \nC.) A second password \nD.) A backup email",   // A
            "A.) Ignore pop-ups asking for personal info \nB.) Click all links that look fun \nC.) Always allow location access \nD.) Use public Wi-Fi for banking" // A
        };

            int[] correctAnwsers = { 2, 0, 2, 0, 3, 3, 1, 1, 0, 0 };
            int playerScore = 0;

            Random r = new Random();
            int rInt = r.Next(0, 19);


        }
    }
}
    