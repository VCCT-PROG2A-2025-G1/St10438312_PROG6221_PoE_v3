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
            WaitingForQuizAnswer,
            WaitingForQuizTypeSelection,
            TryQuizAgain,
            WaitingForSentimentTopic,
            WaitingForFollowUp
        }
        private State _state = State.WaitingForName;

        private string userName;
        private string favTopic;
        private string pendingSentiment;
        private string pendingSentimentTopic;
        private readonly ChatBot_Response _chatBotResponse;
        private readonly CybersecurityQuiz _cybersecurityQuiz;
        private readonly Dictionary<string, int> _sentimentCounts = new Dictionary<string, int>();
        private int _currentQuestionNumber = 0;
        private int _quizScore = 0;
        private const int TotalQuestions = 10;
        private bool _isMultipleChoiceQuiz = false;
        private (string multiChoiceQuestions, string multiChoiceAnwser, int mcCorrectAnwsers) _currentQuizQuestion;
        private (string tfQuestions, string tfAnswers, int tfCorrectAnswers) _currentTFQuizQuestion;

        // Enhanced sentiment detection
        private readonly string[] _positiveWords = { "happy", "good", "great", "excellent", "positive" };
        private readonly string[] _negativeWords = { "bad", "terrible", "awful", "horrible", "negative" };
        private readonly string[] _angryWords = { "angry", "mad", "pissed", "furious", "rage" };
        private readonly string[] _frustratedWords = { "frustrated", "annoyed", "irritated", "upset" };
        private readonly string[] _worriedWords = { "worried", "concerned", "scared", "anxious" };
        private readonly string[] _curiousWords = { "curious", "wonder", "explain", "how", "what", "why" };

        public ChatBot(ChatBot_Response chatBotResponse, CybersecurityQuiz cybersecurityQuiz)
        {
            _chatBotResponse = chatBotResponse;
            _cybersecurityQuiz = cybersecurityQuiz;
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
                    return $"Thanks, {userName}! What is your favorite cybersecurity topic? (phishing, safe browsing, passwords, scams, privacy)";

                case State.WaitingForTopic:
                    favTopic = input.ToLower();
                    _state = State.Chatting;
                    return $"Excellent choice, {userName}! I see you're interested in {favTopic}. " +
                           $"You can now:\n- Ask me anything about cybersecurity\n- Say 'quiz' for a fun test\n- " +
                           $"Share how you're feeling (e.g., 'I'm worried about...')";

                case State.Chatting:
                    if (DetectEnhancedSentiment(input, out string sentiment, out string topic))
                    {
                        if (sentiment == "favourite")
                        {
                            return _chatBotResponse.GetSentimentResponse(favTopic, userName, "favourite");
                        }
                        else
                        {
                            pendingSentiment = sentiment;
                            pendingSentimentTopic = topic ?? DetectTopicFromInput(input);
                            _state = State.WaitingForSentimentTopic;
                            return BuildSentimentPrompt(sentiment, topic);
                        }
                    }
                    else if (input.ToLower().Contains("quiz"))
                    {
                        _state = State.WaitingForQuizTypeSelection;
                        return "What type of quiz would you like?\n1. Multiple Choice\n2. True/False\n\nPlease enter 1 or 2:";
                    }
                    return _chatBotResponse.GetResponse(input, userName);

                case State.WaitingForSentimentTopic:
                    pendingSentimentTopic = string.IsNullOrEmpty(pendingSentimentTopic) ?
                        input.ToLower() : pendingSentimentTopic;

                    string sentimentTopicKey = $"{pendingSentiment}_{pendingSentimentTopic}";
                    _sentimentCounts.TryGetValue(sentimentTopicKey, out int count);
                    _sentimentCounts[sentimentTopicKey] = count + 1;

                    if (count > 3)
                    {
                        _state = State.Chatting;
                        return $"We've discussed {pendingSentimentTopic} several times, {userName}. " +
                               "Maybe we should talk about something else for now?";
                    }

                    string response = _chatBotResponse.GetSentimentResponse(pendingSentimentTopic, userName, pendingSentiment);
                    _state = State.WaitingForFollowUp;
                    return response + "\n\nDid this help address your concerns? (yes/no)";

                case State.WaitingForFollowUp:
                    if (input.ToLower().StartsWith("y"))
                    {
                        _state = State.Chatting;
                        return $"I'm glad to hear that, {userName}! What else can I help with?";
                    }
                    else
                    {
                        _state = State.Chatting;
                        return $"I'm sorry I couldn't help more, {userName}. Maybe try asking differently or choose another topic?";
                    }

                case State.WaitingForQuizTypeSelection:
                    return HandleQuizTypeSelection(input);

                case State.WaitingForQuizAnswer:
                    return ProcessQuizAnswer(input);

                default:
                    _state = State.Chatting;
                    return "Let's continue our conversation. What would you like to know?";
            }
        }

        #region Enhanced Sentiment Detection
        private bool DetectEnhancedSentiment(string input, out string sentiment, out string topic)
        {
            input = input.ToLower();
            sentiment = null;
            topic = null;

            // Check for favorite topic first
            if (!string.IsNullOrEmpty(favTopic) && input.Contains(favTopic))
            {
                sentiment = "favourite";
                return true;
            }

            // Detect sentiment with context
            if (_angryWords.Any(w => input.Contains(w)))
                sentiment = "angry";
            else if (_frustratedWords.Any(w => input.Contains(w)))
                sentiment = "frustrated";
            else if (_worriedWords.Any(w => input.Contains(w)))
                sentiment = "worried";
            else if (_curiousWords.Any(w => input.Contains(w)))
                sentiment = "curious";
            else if (_positiveWords.Any(w => input.Contains(w)))
                sentiment = "positive";
            else if (_negativeWords.Any(w => input.Contains(w)))
                sentiment = "negative";

            // Extract topic from input if sentiment detected
            if (sentiment != null)
            {
                topic = DetectTopicFromInput(input);
                return true;
            }

            return false;
        }

        private string DetectTopicFromInput(string input)
        {
            var topics = new Dictionary<string, string>
        {
            {"phish", "phishing"}, {"password", "passwords"},
            {"brows", "safe browsing"}, {"scam", "scams"},
            {"privac", "privacy"}, {"hack", "hacking"}
        };

            foreach (var pair in topics)
            {
                if (input.Contains(pair.Key))
                    return pair.Value;
            }
            return null;
        }

        private string BuildSentimentPrompt(string sentiment, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                return $"I understand you're feeling {sentiment} about {topic}, {userName}. " +
                       $"Could you tell me more specifically what concerns you?";
            }

            return $"I detect you're feeling {sentiment}, {userName}. " +
                   $"What cybersecurity topic is causing this feeling?";
        }
        #endregion

        #region Quiz Handling
        private string HandleQuizTypeSelection(string input)
        {
            if (input == "1")
            {
                _isMultipleChoiceQuiz = true;
                _currentQuestionNumber = 0;
                _quizScore = 0;
                _currentQuizQuestion = CybersecurityQuiz.GetMCQuestionByIndex(0);

                _state = State.WaitingForQuizAnswer;
                return FormatMCQuestion(1, _currentQuizQuestion);
            }
            else if (input == "2")
            {
                _isMultipleChoiceQuiz = false;
                _currentQuestionNumber = 0;
                _quizScore = 0;
                _currentTFQuizQuestion = CybersecurityQuiz.GetTFQuestionByIndex(0);

                _state = State.WaitingForQuizAnswer;
                return FormatTFQuestion(1, _currentTFQuizQuestion);
            }
            return "Please enter either 1 for Multiple Choice or 2 for True/False:";
        }

        private string ProcessQuizAnswer(string input)
        {
            if (_isMultipleChoiceQuiz)
            {
                int answerIndex = ConvertAnswerToIndex(input);
                if (answerIndex == -1)
                    return "Invalid input. Please answer with A, B, C, or D:";

                bool correct = answerIndex == _currentQuizQuestion.mcCorrectAnwsers;
                if (correct) _quizScore++;

                return HandleQuizTransition(correct);
            }
            else
            {
                int answerIndex = ConvertTFAnswerToIndex(input);
                if (answerIndex == -1)
                    return "Invalid input. Please answer with A or B:";

                bool correct = answerIndex == _currentTFQuizQuestion.tfCorrectAnswers;
                if (correct) _quizScore++;

                return HandleQuizTransition(correct);
            }
        }

        private string HandleQuizTransition(bool correct)
        {
            _currentQuestionNumber++;

            if (_currentQuestionNumber >= TotalQuestions)
            {
                _state = State.Chatting;
                return $"Quiz complete! Final score: {_quizScore}/{TotalQuestions}. " +
                       $"Type 'quiz' to try again or ask me anything else, {userName}!";
            }

            string feedback = correct ? "Correct!" : "Incorrect!";
            string nextQuestion;

            if (_isMultipleChoiceQuiz)
            {
                _currentQuizQuestion = CybersecurityQuiz.GetMCQuestionByIndex(_currentQuestionNumber);
                nextQuestion = FormatMCQuestion(_currentQuestionNumber + 1, _currentQuizQuestion);
            }
            else
            {
                _currentTFQuizQuestion = CybersecurityQuiz.GetTFQuestionByIndex(_currentQuestionNumber);
                nextQuestion = FormatTFQuestion(_currentQuestionNumber + 1, _currentTFQuizQuestion);
            }

            return $"{feedback} Current score: {_quizScore}/{_currentQuestionNumber}\n\n{nextQuestion}";
        }

        private string FormatMCQuestion(int number, (string q, string a, int correct) question)
        {
            return $"Question {number}:\n{question.q}\n\n{question.a}\nPlease answer with A, B, C, or D:";
        }

        private string FormatTFQuestion(int number, (string q, string a, int correct) question)
        {
            return $"Question {number}:\n{question.q}\n\n{question.a}\nPlease answer with A (True) or B (False):";
        }

        private int ConvertAnswerToIndex(string input)
        {
            if (string.IsNullOrEmpty(input)) return -1;
            char c = char.ToUpper(input[0]);
            return c >= 'A' && c <= 'D' ? c - 'A' : -1;
        }

        private int ConvertTFAnswerToIndex(string input)
        {
            if (string.IsNullOrEmpty(input)) return -1;
            char c = char.ToUpper(input[0]);
            return c == 'A' ? 0 : c == 'B' ? 1 : -1;
        }
        #endregion
    }
}

//------------------------------------------------------------------------------------------------------------------------//

//-------------------------------------------------END OF FILE-------------------------------------------------//

