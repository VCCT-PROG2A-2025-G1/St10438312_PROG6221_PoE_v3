using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10438312_PROG6221_PoE_v3
{
    //-------------------------------------------------START OF FILE-------------------------------------------------//
    internal class ChatBot_Response
    {
        // Helper class that groups responses by sentiment
        private class TopicResponses
        {
            public List<Func<string, string>> Worried { get; set; } = new List<Func<string, string>>();
            public List<Func<string, string>> Curious { get; set; } = new List<Func<string, string>>();
            public List<Func<string, string>> Frustrated { get; set; } = new List<Func<string, string>>();
            public List<Func<string, string>> Favourite { get; set; } = new List<Func<string, string>>();

        }

        // Tone detection word banks
        private readonly string[] worriedWords = { "worried", "concerned", "scared", "anxious", "nervous", "afraid" };
        private readonly string[] frustratedWords = { "frustrated", "annoyed", "irritated", "upset", "sick of", "tired of" };
        private readonly string[] angryWords = { "angry", "mad", "pissed", "furious", "rage", "livid" };
        private readonly string[] curiousWords = { "curious", "wonder", "explain", "how", "what", "why", "tell me" };

        private readonly Dictionary<string, List<Func<string, string>>> responses;
        private readonly Dictionary<string, TopicResponses> sentimentResponses;
        private string lastTopic = null;
        private readonly Random random = new Random();


        //------------------------------------------------------------------------------------------------------------------------//
        public ChatBot_Response()
        {
            responses = ResponseList();
            sentimentResponses = SentimentResponses();
        }
        //------------------------------------------------------------------------------------------------------------------------//



        //------------------------------------------------------------------------------------------------------------------------//
        // Dictionary of responses
        private Dictionary<string, List<Func<string, string>>> ResponseList()
        {
            return new Dictionary<string, List<Func<string, string>>>(StringComparer.OrdinalIgnoreCase)
            {
                // Chatbot greeting responses
                { "hello", new List<Func<string, string>>
                    {
                    userName => $"Hello there, {userName}! How can I help you with cybersecurity today?",
                    userName => $"Hi {userName}, welcome back! What would you like to know about cybersecurity?",
                    userName => $"Greetings, {userName}! Ask me anything about online safety."
                    }
                },

                { "hi", new List<Func<string, string>>
                {
                    userName => $"Hello, {userName}! Are you ready to learn about cybersecurity today?",
                    userName => $"Hi {userName}, welcome back! I cant wait to help you with cybersecurity",
                    userName => $"Greetings, {userName}! Ask me anything about online safety."
                    }
                },

                { "how are you", new List<Func<string, string>>
                {
                    userName => $"Im doing great {userName}! Im hope everything is well",
                    userName => $"Its always going good with you around, {userName}.",
                    userName => $"When I help people with cybersecurity, its alway going good {userName}!"
                    }
                },

                { "what is your purpose", new List<Func<string, string>>
                {
                    userName => $"My purpose is to help educate people like you, {userName}, about cybersecurity.",
                    userName => $"I am here to bring awareness to cybersecurity, {userName}",
                    userName => $"My purpose is to educate  {userName}!"
                    }
                },

                { "what can i ask you about", new List<Func<string, string>>
                {
                    userName => $"{userName}, you can ask me about password safety, phishing attacks, safe browsing and general cybersecurity.",
                    userName => $"I can help with passwords, phishing, safe browsing, and more cybersecurity topics, {userName}!",
                    userName => $"Feel free to ask about cybersecurity topics like phishing, passwords, or safe browsing, {userName}."
                    }
                },

    
                // Phishing
                { "phishing", new List<Func<string, string>>
                {
                    userName => $"Phishing is a type of scam where attackers trick you into giving personal info like passwords. Always verify the sender and avoid clicking suspicious links, {userName}.",
                    userName => $"Phishing attacks use fake emails or websites to steal your information, {userName}. Be very careful with unexpected messages!",
                    userName => $"Watch out for phishing, {userName}! Cybercriminals use deceptive messages to trick people into sharing sensitive data."
                    }
                },

                { "what is phishing", new List<Func<string, string>>
                {
                    userName => $"Phishing is a method used by attackers to steal your data through fake emails or websites. Be cautious, {userName}, and don't share sensitive information.",
                    userName => $"Phishing is when criminals impersonate trusted sources to trick you into revealing personal information, {userName}.",
                    userName => $"It's a cyber attack where scammers pose as legitimate organizations to steal your credentials, {userName}."
                    }
                },
                { "how to avoid phishing", new List<Func<string, string>>
                {
                    userName => $"To avoid phishing, {userName}, don't click on unknown links, double-check email addresses, and never give out personal information unless you're sure it's safe.",
                    userName => $"Stay safe from phishing by verifying sender identity, avoiding suspicious attachments, and using two-factor authentication, {userName}.",
                    userName => $"Protect yourself by being skeptical of urgent requests, checking URLs carefully, and keeping your software updated, {userName}."
                    }
                },

                // Passwords
                { "passwords", new List<Func<string, string>>
                {
                    userName => $"Using strong, unique passwords is key, {userName}. Consider using a password manager to keep them safe.",
                    userName => $"Password security is crucial, {userName}! Make sure each account has a different, complex password.",
                    userName => $"Good password practices can protect all your accounts, {userName}. Never use the same password twice!"
                    }
                },

                { "password safety", new List<Func<string, string>>
                {
                    userName => $"Password safety tip, {userName}: use at least 12 characters, mix letters, numbers, symbols, and avoid using the same password on multiple sites.",
                    userName => $"For password safety, {userName}, create long, random passwords and enable two-factor authentication wherever possible.",
                    userName => $"Keep your passwords safe by making them complex, unique, and storing them securely, {userName}."
                    }
                },

                { "what is a strong password", new List<Func<string, string>>
                {
                    userName => $"A strong password should be at least 12 characters long with a mix of upper and lowercase letters, numbers, and special characters, {userName}.",
                    userName => $"Strong passwords are long, random, and include various character types, {userName}. Think of it like a complex lock!",
                    userName => $"The best passwords are unique, lengthy, and unpredictable combinations of letters, numbers, and symbols, {userName}."
                    }
                },

                { "should i reuse passwords", new List<Func<string, string>>
                {
                    userName => $"No, {userName}. Reusing passwords increases your risk. If one account is compromised, others could be too.",
                    userName => $"Definitely not, {userName}! Each account should have its own unique password to prevent chain reactions if one gets hacked.",
                    userName => $"Password reuse is dangerous, {userName}. One breach could lead to multiple account compromises."
                    }
                },

                // Safe Browsing
                { "safe browsing", new List<Func<string, string>>
                {
                    userName => $"Safe browsing means avoiding risky websites, keeping your browser updated, and using security extensions, {userName}.",
                    userName => $"Browse safely by sticking to trusted sites, using HTTPS connections, and being cautious with downloads, {userName}.",
                    userName => $"Safe browsing protects you from malware and scams, {userName}. Always verify website authenticity!"
                    }
                },

                { "how to browse safely", new List<Func<string, string>>
                {
                    userName => $"To browse safely, {userName}, avoid clicking suspicious links, use HTTPS websites, keep your browser updated, and use a good antivirus.",
                    userName => $"Browse safely by using reputable browsers, enabling security features, and avoiding pop-ups or suspicious downloads, {userName}.",
                    userName => $"Stay safe online by checking URLs, using ad blockers, and never downloading software from untrusted sources, {userName}."
                    }
                },

                { "what is safe browsing", new List<Func<string, string>>
                {
                    userName => $"Safe browsing is all about protecting yourself online by avoiding dangerous websites and using privacy tools, {userName}.",
                    userName => $"It's the practice of navigating the internet securely while protecting your personal information, {userName}.",
                    userName => $"Safe browsing means being aware of online threats and taking steps to avoid malicious websites, {userName}."
                    }
                },

                // Help command
                { "help", new List<Func<string, string>>
                {
                    userName =>
                     "Here is a list of the things I can help with:" +
                      "\n\nPhishing:" +
                      "\n - What is phishing" +
                      "\n - How to avoid phishing" +
                      "\nPasswords:" +
                      "\n - Password safety" +
                      "\n - What is a strong password" +
                      "\n - Should I reuse passwords" +
                      "\nSafe Browsing:" +
                      "\n - What is safe browsing" +
                      "\n - How to browse safely",

                    userName =>
                     $"Hi {userName}! I can assist you with:" +
                      "\n\n Phishing Protection" +
                      "\n Password Security" +
                      "\n Safe Browsing Tips" +
                      "\n\nJust ask me about any of these topics!",

                    userName =>
                     $"Need help, {userName}? I specialize in:" +
                      "\n• Identifying and avoiding phishing attacks" +
                      "\n• Creating strong, secure passwords" +
                      "\n• Browsing the internet safely" +
                      "\n\nWhat would you like to learn about?"
                      }
                }
            };
        }


        //------------------------------------------------------------------------------------------------------------------------//
        private Dictionary<string, TopicResponses> SentimentResponses()
        {
            // The dictionary maps a keyword to a TopicResponse object. 
            //TopicResponse object contains multiple lists (e.g. Worried, Curious, Frustrated) that each hold responses for that sentiment 
            return new Dictionary<string, TopicResponses>(StringComparer.OrdinalIgnoreCase)
            {
                // Phishing
                ["phishing"] = new TopicResponses
                {
                    Worried = new List<Func<string, string>>
                    {
                        userName => $"{userName}, I understand phishing feels scary, but you're already ahead of the game by being aware of it! Most phishing attempts are actually quite obvious once you know what to look for. Trust your instincts - if something feels off about an email or message, it probably is. You've got this!",
                        userName => $"Take a deep breath {userName} - phishing attacks rely on panic and rushed decisions. The good news is that simply being cautious and taking a moment to verify suspicious messages puts you in control. Your awareness is your best defense, and that's something you already have.",
                        userName => $"Don't worry {userName}, phishing fears shouldn't keep you from enjoying the internet safely. Think of it like crossing the street - once you learn to look both ways, it becomes second nature. You're building great digital habits, and that confidence will serve you well."
                    },
                    Curious = new List<Func<string, string>>
                    {
                        userName => $"Great question {userName}! Phishing is when cybercriminals try to trick you into revealing personal information by pretending to be legitimate organizations. They usually do this through fake emails, texts, or websites that look real but are designed to steal your data.",
                        userName => $"Phishing attacks are fascinating from a security perspective {userName}. They exploit human psychology rather than technical vulnerabilities - attackers create urgency, fear, or excitement to make people act without thinking. The best defense is simply taking a moment to verify before clicking or sharing information.",
                        userName => $"You're smart to ask about this {userName}! Modern phishing attempts can be quite sophisticated, using personal information from social media to make their messages seem more legitimate. However, there are always telltale signs like urgent language, suspicious links, or requests for sensitive information that legitimate companies wouldn't ask for via email."
                    },
                    Frustrated = new List<Func<string, string>>
                    {
                        userName => $"I totally get your frustration {userName} - dealing with constant phishing attempts is exhausting! The good news is that reporting these emails to your email provider helps improve their filters for everyone. Most email services have gotten much better at catching these automatically.",
                        userName => $"You're right to be annoyed {userName} - phishing emails are incredibly persistent and annoying. Consider using email filters or switching to an email provider with stronger spam protection. Gmail, Outlook, and others have significantly improved their phishing detection in recent years.",
                        userName => $"I hear you {userName} - it's maddening when these fake emails keep slipping through! Try marking them as phishing/spam rather than just deleting them. This trains your email system to recognize similar attempts. Also, never reply or click 'unsubscribe' on obvious phishing emails as this just confirms your email is active."
                    },
                    Favourite = new List<Func<string, string>>
                    {
                        userName => $"I remember you said your favourite topic is phishing {userName}! Here are some special facts about it just for you! Did you know that the term 'phishing' comes from 'fishing' - cybercriminals cast out bait (fake emails) hoping someone will bite? The first recorded phishing attack was in the 1990s targeting AOL users. Modern phishing has evolved incredibly - some attacks now use AI to create personalized messages based on your social media posts, making them seem eerily legitimate. The most sophisticated phishing attempts can even create fake websites that are nearly identical to real ones, down to the SSL certificates!"
                    }
                },

                // Password
                ["password"] = new TopicResponses
                {
                    Worried = new List<Func<string, string>>
                    {
                        userName => $"Password security doesn't have to be overwhelming {userName}! You don't need to memorize dozens of complex passwords - that's what password managers are for. Start with just securing your most important accounts, and you'll feel much more in control.",
                        userName => $"Breathe easy {userName} - even if your passwords aren't perfect right now, taking small steps makes a huge difference. Updating just one or two important passwords today puts you way ahead of most people. Progress over perfection!",
                        userName => $"Your concern about passwords shows you're taking security seriously {userName}, which is wonderful! Remember, you only need to remember one strong master password if you use a password manager. Everything else can be automatically generated and stored securely."
                    },
                    Curious = new List<Func<string, string>>
                    {
                        userName => $"Passwords are your first line of defense {userName}! A strong password should be at least 12 characters long and include a mix of letters, numbers, and symbols. But honestly, using a password manager to generate and store unique passwords for each account is the way to go.",
                        userName => $"Interesting question {userName}! The most important thing about passwords isn't complexity - it's uniqueness. Using the same password across multiple sites is like using the same key for your house, car, and office. If one gets compromised, everything is at risk.",
                        userName => $"You're thinking about this the right way {userName}! Two-factor authentication is actually more important than having super complex passwords. Even if someone gets your password, they still can't access your account without that second verification step from your phone."
                    },
                    Frustrated = new List<Func<string, string>>
                    {
                        userName => $"I understand the password fatigue {userName} - it's genuinely annoying to manage so many different logins! This is exactly why password managers exist. They eliminate the mental burden while keeping you secure. Most can auto-fill your passwords too.",
                        userName => $"You're absolutely right {userName} - password requirements can be ridiculous and inconsistent across different sites! Focus on using a password manager that can handle all these quirky requirements automatically. Let technology solve this technology problem.",
                        userName => $"The constant password resets are infuriating {userName}, I get it! Consider consolidating accounts where possible and definitely invest in a good password manager. Also, enable account recovery options like backup emails or phone numbers to make resets easier when they do happen."
                    },
                    Favourite = new List<Func<string, string>>
                    {
                        userName => $"I remember you said your favourite topic is passwords {userName}! Here are some special facts about it just for you! The world's most common password is still '123456' - used by millions of people worldwide! Password strength isn't just about complexity though - length matters more than you'd think. A 12-character password with simple words is actually stronger than an 8-character password with special symbols. Some of the most secure organizations use passphrases instead of passwords - think 'correct horse battery staple' rather than 'Tr0ub4dor&3'. The human brain can actually remember longer, story-like passwords better than short, complex ones. That's why security experts now recommend memorable phrases over cryptic character combinations!"
                    }
                },

                // Safe Browsing
                ["safe browsing"] = new TopicResponses
                {
                    Worried = new List<Func<string, string>>
                    {
                        userName => $"Safe browsing is much simpler than it seems {userName}! Your browser already blocks most dangerous sites automatically. Stick to well-known websites for important activities, and you're already doing great. Trust yourself to make smart choices online.",
                        userName => $"You don't need to be afraid of the internet {userName} - just mindful! Think of safe browsing like driving carefully: stay alert, but don't let worry ruin the journey. Your common sense is your best navigation tool.",
                        userName => $"Most websites you visit daily are perfectly safe {userName}! The internet isn't as dangerous as headlines make it seem. Focus on enjoying the amazing resources available online while using basic caution, just like you would in any environment."
                    },
                    Curious = new List<Func<string, string>>
                    {
                        userName => $"Safe browsing is all about being aware of potential risks {userName}! Look for HTTPS (the lock icon) on websites where you enter personal information, avoid downloading software from unknown sources, and be cautious about clicking suspicious links or pop-ups.",
                        userName => $"Good question {userName}! Modern browsers like Chrome, Firefox, and Edge have built-in protections that warn you about malicious websites. They maintain databases of known dangerous sites and will block them automatically. Your browser is working hard to keep you safe behind the scenes!",
                        userName => $"You're smart to learn about this {userName}! Safe browsing also means keeping your browser updated, using reputable antivirus software, and being selective about browser extensions. Most security issues come from outdated software or sketchy downloads rather than just visiting websites."
                    },
                    Frustrated = new List<Func<string, string>>
                    {
                        userName => $"Those constant security warnings can be really annoying {userName}! Most of them are actually helpful, but you can adjust your browser's security settings if they're too intrusive. Just make sure you understand what you're disabling before making changes.",
                        userName => $"I hear your frustration {userName} - some websites are overly aggressive with their security measures! If you're consistently having issues with legitimate sites, try clearing your browser cache and cookies, or temporarily disable ad blockers to see if that helps.",
                        userName => $"Website security can definitely be a pain {userName}! If you're getting blocked from sites you trust, try using a different browser or incognito/private mode. Sometimes browser extensions or strict security settings can cause issues with perfectly safe websites."
                    },
                    Favourite = new List<Func<string, string>>
                    {
                        userName => $"I remember you said your favourite topic is safe browsing {userName}! Here are some special facts about it just for you! Your browser is like a digital bodyguard - it checks thousands of websites against security databases every day without you even knowing. Chrome alone blocks over 100 million malicious sites daily! Browsers have a 'sandbox' feature that isolates each tab - so if one tab gets compromised, it can't affect the others or your computer. Modern browsers can actually predict and preload safe websites you're likely to visit next, speeding up your browsing while keeping security checks running in the background. Incognito mode doesn't just hide your browsing from other people on your computer - it also provides some additional security by not storing cookies or site data that could be exploited later!"
                    }
                },

                // Scam
                ["scam"] = new TopicResponses
                {
                    Worried = new List<Func<string, string>>
                    {
                        userName => $"Scammers count on fear and confusion {userName}, but you're already winning by staying informed! Remember, legitimate companies will never pressure you to act immediately or ask for sensitive information unexpectedly. Your skepticism is a superpower.",
                        userName => $"Take comfort in knowing that scams often follow predictable patterns {userName}. Once you recognize the common signs - urgency, too-good-to-be-true offers, requests for personal info - they become much easier to spot and avoid. You're building excellent judgment!",
                        userName => $"Don't let scam anxiety keep you from beneficial opportunities online {userName}. Most businesses and services are legitimate and helpful. Trust your instincts, take time with important decisions, and remember that being cautious doesn't mean being fearful."
                    },
                    Curious = new List<Func<string, string>>
                    {
                        userName => $"Scams come in many forms {userName}, but they all try to manipulate emotions - fear, greed, urgency, or sympathy. Common ones include fake tech support calls, romance scams, lottery/prize notifications, and fake charity requests. The key is recognizing these emotional manipulation tactics.",
                        userName => $"That's a great question {userName}! Modern scams are increasingly sophisticated, using AI and personal information from social media to seem more legitimate. However, they still rely on the same basic principles: creating urgency and asking for money or personal information before you can think it through.",
                        userName => $"You're wise to educate yourself {userName}! The most effective scams target people during vulnerable moments - after a death, during financial stress, or when they're excited about an opportunity. Scammers are skilled at reading situations and adapting their approach accordingly."
                    },
                    Frustrated = new List<Func<string, string>>
                    {
                        userName => $"I completely understand your anger {userName} - scams are violating and infuriating! Channel that frustration into protection: report scams to the FTC, warn friends and family about new tactics you encounter, and remember that your vigilance helps protect others too.",
                        userName => $"Your frustration is totally justified {userName} - it's maddening how persistent and creative scammers can be! The best revenge is staying informed and helping others avoid the same traps. Consider it a victory every time you recognize and avoid a scam attempt.",
                        userName => $"I get it {userName} - dealing with constant scam attempts is exhausting and offensive! Focus on the fact that your awareness makes you a hard target. Scammers move on to easier victims when they encounter informed, skeptical people like yourself."
                    }
                },

                // Privacy
                ["privacy"] = new TopicResponses
                {
                    Worried = new List<Func<string, string>>
                    {
                        userName => $"Protecting your privacy online is totally manageable {userName}! Start with the basics - adjust your social media privacy settings and be mindful of what you share. Small changes make a big difference, and you don't need to go off the grid completely.",
                        userName => $"Your privacy concerns show great digital awareness {userName}! Remember, you have more control than you think. Use privacy-focused browsers, review app permissions occasionally, and know that being selective about what you share online is perfectly normal and smart.",
                        userName => $"Don't let privacy worries overwhelm you or keep you from enjoying online services {userName}. Many companies actually work hard to protect user data because it's good business. Focus on the privacy settings you can control and trust yourself to make informed choices."
                    },
                    Curious = new List<Func<string, string>>
                    {
                        userName => $"Privacy online is about controlling what information you share and with whom {userName}. This includes personal details, browsing habits, location data, and more. Companies use this data for advertising, product improvement, and sometimes sell it to third parties - though regulations like GDPR are changing this.",
                        userName => $"Great question {userName}! Your digital privacy involves everything from the websites you visit to the apps you use. Most services collect data to improve their products and show relevant ads. The key is understanding what's being collected and making informed choices about what you're comfortable sharing.",
                        userName => $"You're right to be curious {userName}! Digital privacy is complex because it involves balancing convenience with control. Features like personalized recommendations require data sharing, but you can often adjust settings to limit what's collected while still enjoying the benefits of online services."
                    },
                    Frustrated = new List<Func<string, string>>
                    {
                        userName => $"I totally get your privacy frustration {userName} - the constant tracking and data collection can feel invasive! Focus on what you can control: use privacy-focused browsers like Firefox or Brave, regularly review app permissions, and consider privacy-focused alternatives for services you use most.",
                        userName => $"Your frustration with privacy violations is completely valid {userName}! The good news is that regulations are getting stronger and companies are being held more accountable. In the meantime, tools like ad blockers, VPNs, and privacy settings can give you back some control.",
                        userName => $"I hear you {userName} - the lack of transparency around data collection is genuinely infuriating! Channel that energy into taking action: read privacy policies of services you care about, use privacy tools, and support companies that prioritize user privacy. Your choices make a difference."
                    }

                }
            };
        }


        //------------------------------------------------------------------------------------------------------------------------//
        // Method to retrieve chatbot response based on user input
        public string GetResponse(string userInput, string userName)
        {
            userInput = userInput.Trim().ToLower();

            // Then check for known response keys
            foreach (var key in responses.Keys)
            {
                if (userInput.Contains(key.ToLower()))
                {
                    var responseList = responses[key];
                    var responseFunc = responseList[random.Next(responseList.Count)];
                    return responseFunc(userName);
                }
            }
            return $"I'm not sure how to respond to that, {userName}. Type 'help' for a list of topics I can assist with.";
        }
        //------------------------------------------------------------------------------------------------------------------------//


        public string GetSentimentResponse(string userInput, string userName, string tone)
        {
            // Normalize both inputs
            userInput = userInput.Trim().ToLower();
            tone = tone.ToLower();

            // Goes through all topics in sentimentResponse an checks if the userInput contains that word
            foreach (var key in sentimentResponses.Keys)
            {
                if (userInput.Contains(key.ToLower()))
                {
                    // Gets topic object associated with keyword and sets it to be used later e.g. will get "password"
                    var topic = sentimentResponses[key];

                    // Selector vaiable - Creates a refrence point with no list. Switch statement will assign the actual list
                    List<Func<string, string>> responseList = null;

                    // Switch statement selects the approprate response list based on the users tone
                    switch (tone)
                    {
                        case "worried":
                            responseList = topic.Worried;
                            break;
                        case "frustrated":
                            responseList = topic.Frustrated;
                            break;
                        case "curious":
                            responseList = topic.Curious;
                            break;
                        case "favourite":
                            responseList = topic.Favourite;
                            break;
                    }

                    // If a valid response list exists, it will randomly select one function 
                    // Checks to see if responseList has been assigned and if there are values or "responses" in the list
                    if (responseList != null && responseList.Count > 0)
                    {
                        var responseFunc = responseList[random.Next(responseList.Count)];
                        return responseFunc(userName);
                    }
                }
            }
            return $"I'm not sure how to respond to that, {userName}. Type 'help' for a list of topics I can assist with.";
        }

    }

    //-------------------------------------------------END OF FILE-------------------------------------------------//
}
