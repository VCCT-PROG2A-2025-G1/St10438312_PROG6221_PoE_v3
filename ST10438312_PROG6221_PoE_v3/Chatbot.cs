using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10438312_PROG6221_PoE_v3
{
    //-------------------------------------------------END OF FILE-------------------------------------------------//
    internal class ChatBot
    {
        private enum State
        {
            WaitingForName,
            WaitingForTopic,
            Chatting,
            WaitingForSentimentTopic,
            WaitingForFollowUp
        }
        private State _state = State.WaitingForName;

        private string userName;
        private string favTopic;
        private string pendingSentiment; // Stores the detected sentiment
        private string pendingSentimentTopic; // Stores the topic for follow-up
        private readonly ChatBot_Response _chatBotResponse;
        private readonly Dictionary<string, int> _sentimentCounts = new Dictionary<string, int>();

        public ChatBot(ChatBot_Response chatBotResponse)
        {
            _chatBotResponse = chatBotResponse;
        }

        public string StartUpMessage()
        {
            _state = State.WaitingForName;
            return "Hello and Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online!\n\nBefore we start, please enter your name:";
        }

        public string ProcessUserInput(string input)
        {
            input = input?.Trim();
            if (string.IsNullOrEmpty(input))
                return "Please enter something!";

            switch (_state)
            {
                case State.WaitingForName:
                    userName = input;
                    _state = State.WaitingForTopic;
                    return $"Thanks, {userName}! What is your favorite topic? (phishing, safe browsing, passwords)";

                case State.WaitingForTopic:
                    favTopic = input.ToLower();
                    _state = State.Chatting;
                    return $"Great! You can now chat with me about cybersecurity, {userName}.";

                case State.Chatting:
                    // Check for sentiment keywords or favorite topic
                    if (ContainsSentiment(input, out string sentiment))
                    {
                        if (sentiment == "favourite")
                        {
                            // Respond immediately with favourite sentiment
                            return _chatBotResponse.GetSentimentResponse(favTopic, userName, "favourite");
                        }
                        else
                        {
                            pendingSentiment = sentiment;
                            _state = State.WaitingForSentimentTopic;
                            return $"What are you {sentiment} about, {userName}?";
                        }
                    }
                    // Normal chat
                    return _chatBotResponse.GetResponse(input, userName);

                case State.WaitingForSentimentTopic:
                    pendingSentimentTopic = input.ToLower();
                    string sentimentTopicKey = $"{pendingSentiment}_{pendingSentimentTopic}";
                    if (!_sentimentCounts.ContainsKey(sentimentTopicKey))
                        _sentimentCounts[sentimentTopicKey] = 0;
                    _sentimentCounts[sentimentTopicKey]++;

                    if (_sentimentCounts[sentimentTopicKey] > 6)
                    {
                        _state = State.Chatting;
                        return $"I've tried to help you with being {pendingSentiment} about {pendingSentimentTopic} several times now. I think I've done as much as I can on this topic.";
                    }

                    string sentimentResponse = _chatBotResponse.GetSentimentResponse(pendingSentimentTopic, userName, pendingSentiment);
                    _state = State.WaitingForFollowUp;
                    return sentimentResponse + "\n\nDid that help at all? (yes/no)";

                case State.WaitingForFollowUp:
                    string lower = input.ToLower();
                    if (lower.Contains("yes") || lower.Contains("better"))
                    {
                        _state = State.Chatting;
                        return "I'm happy to hear that!";
                    }
                    else if (lower.Contains("no") || lower.Contains("not"))
                    {
                        _state = State.Chatting;
                        return "I'm sorry I couldn't help more. Feel free to ask again and maybe I can help this time.";
                    }
                    else
                    {
                        return "I'm not sure I understand. Do you feel better? Please answer yes or no.";
                    }

                default:
                    _state = State.Chatting;
                    return "Something went wrong. Let's continue chatting!";
            }
        }

        private bool ContainsSentiment(string input, out string sentiment)
        {
            input = input.ToLower();
            if (!string.IsNullOrEmpty(favTopic) && input.Contains(favTopic))
            {
                sentiment = "favourite";
                return true;
            }
            if (input.Contains("worried"))
            {
                sentiment = "worried";
                return true;
            }
            if (input.Contains("curious"))
            {
                sentiment = "curious";
                return true;
            }
            if (input.Contains("frustrated"))
            {
                sentiment = "frustrated";
                return true;
            }
            sentiment = null;
            return false;
        }
    }
}

//------------------------------------------------------------------------------------------------------------------------//

//-------------------------------------------------END OF FILE-------------------------------------------------//

