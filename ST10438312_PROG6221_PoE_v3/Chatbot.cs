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
        private string pendingSentiment; // Stores the detected sentiment
        private string pendingSentimentTopic; // Stores the topic for follow-up
        private readonly ChatBot_Response _chatBotResponse;
        private readonly CybersecurityQuiz _cybersecurityQuiz;
        private readonly Dictionary<string, int> _sentimentCounts = new Dictionary<string, int>();
        private int _currentQuestionNumber = 0;
        private int _quizScore = 0;
        private const int TotalQuestions = 10;
        private bool _isMultipleChoiceQuiz = false;
        private (string multiChoiceQuestions, string multiChoiceAnwser, int mcCorrectAnwsers) _currentQuizQuestion;
        private (string tfQuestions, string tfAnswers, int tfCorrectAnswers) _currentTFQuizQuestion;



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
                //---------------------------------------------------------------------------//
                case State.WaitingForName:
                    userName = input;
                    _state = State.WaitingForTopic;
                    return $"Thanks, {userName}! What is your favorite topic? (phishing, safe browsing, passwords)";
                //---------------------------------------------------------------------------//



                //---------------------------------------------------------------------------//
                case State.WaitingForTopic:
                    favTopic = input.ToLower();
                    _state = State.Chatting;
                    return $"Great! You can now chat with me about cybersecurity, {userName}.";
                //---------------------------------------------------------------------------//


                //---------------------------------------------------------------------------//
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

                    else if (input.ToLower().Contains("quiz"))
                    {
                        _state = State.WaitingForQuizTypeSelection;
                        return "What type of quiz would you like?\n1. Multiple Choice\n2. True/False\n\nPlease enter 1 or 2:";
                    }

                    // Normal chat
                    return _chatBotResponse.GetResponse(input, userName);
                //---------------------------------------------------------------------------//


                //---------------------------------------------------------------------------//
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
                //---------------------------------------------------------------------------//


                //---------------------------------------------------------------------------//
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
                //---------------------------------------------------------------------------//


                case State.WaitingForQuizTypeSelection:
                    if (input == "1")
                    {
                        _isMultipleChoiceQuiz = true;
                        _currentQuestionNumber = 0;
                        _quizScore = 0;

                        // Load first question here
                        _currentQuizQuestion = CybersecurityQuiz.GetMCQuestionByIndex(_currentQuestionNumber);
                        _state = State.WaitingForQuizAnswer;

                        return $"Question 1:\n{_currentQuizQuestion.multiChoiceQuestions}\n\n{_currentQuizQuestion.multiChoiceAnwser}\nPlease answer with A, B, C, or D:";
                    }
                    else if (input == "2")
                    {
                        _isMultipleChoiceQuiz = false;
                        _currentQuestionNumber = 0;
                        _quizScore = 0;

                        // Load first question here
                        _currentTFQuizQuestion = CybersecurityQuiz.GetTFQuestionByIndex(_currentQuestionNumber);
                        _state = State.WaitingForQuizAnswer;  

                        return $"Question 1:\n{_currentTFQuizQuestion.tfQuestions}\n\n{_currentTFQuizQuestion.tfAnswers}\nPlease answer with A (True) or B (False):";
                    }
                    else
                    {
                        return "Please enter either 1 for Multiple Choice or 2 for True/False:";
                    }


                case State.WaitingForQuizAnswer:
                    int userAnswerIndex;

                    if (_isMultipleChoiceQuiz)
                    {
                        userAnswerIndex = ConvertAnswerToIndex(input);

                        if (userAnswerIndex == -1)
                            return "Invalid input. Please answer with A, B, C, or D:";

                        bool correct = userAnswerIndex == _currentQuizQuestion.mcCorrectAnwsers;
                        if (correct) _quizScore++;

                        _currentQuestionNumber++;

                        if (_currentQuestionNumber >= TotalQuestions)
                        {
                            _state = State.Chatting;
                            return $"Your final score is {_quizScore}/{TotalQuestions}.\nThanks for playing! Type 'quiz' to try again.";
                        }
                        else
                        {
                            _currentQuizQuestion = CybersecurityQuiz.GetMCQuestionByIndex(_currentQuestionNumber);
                            string feedback = correct ? "That's right!" : "That's incorrect.";
                            return $"{feedback} Your score so far is {_quizScore}/{_currentQuestionNumber}.\n\n" +
                                   $"Question {_currentQuestionNumber + 1}:\n{_currentQuizQuestion.multiChoiceQuestions}\n\n{_currentQuizQuestion.multiChoiceAnwser}\nPlease answer with A, B, C, or D:";
                        }
                    }

                    else
                    {
                        userAnswerIndex = ConvertTFAnswerToIndex(input);
                        if (userAnswerIndex == -1)
                            return "Invalid input. Please answer with A or B:";

                        bool correct = userAnswerIndex == _currentTFQuizQuestion.tfCorrectAnswers;
                        if (correct) _quizScore++;

                        _currentQuestionNumber++;

                        if (_currentQuestionNumber >= TotalQuestions)
                        {
                            _state = State.Chatting;
                            return $"Your final score is {_quizScore}/{TotalQuestions}.\nThanks for playing! Type 'quiz' to try again.";
                        }
                        else
                        {
                            _currentTFQuizQuestion = CybersecurityQuiz.GetTFQuestionByIndex(_currentQuestionNumber);
                            string feedback = correct ? "That's right!" : "That's incorrect.";
                            return $"{feedback} Your score so far is {_quizScore}/{_currentQuestionNumber}.\n\n" +
                                   $"Question {_currentQuestionNumber + 1}:\n{_currentTFQuizQuestion.tfQuestions}\n\n{_currentTFQuizQuestion.tfAnswers}\nPlease answer with A (True) or B (False):";
                        }
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


        private string AskNextQuizQuestion()
        {
            if (_isMultipleChoiceQuiz)
            {
                _currentQuizQuestion = CybersecurityQuiz.GetMCQuestionByIndex(_currentQuestionNumber);
                return $"Question {_currentQuestionNumber + 1}/{TotalQuestions}:\n{_currentQuizQuestion.multiChoiceQuestions}\n" +
                       $"\n{_currentQuizQuestion.multiChoiceAnwser}\nPlease answer with A, B, C, or D:";
            }
            else
            {
                _currentTFQuizQuestion = CybersecurityQuiz.GetTFQuestionByIndex(_currentQuestionNumber);
                return $"Question {_currentQuestionNumber + 1}/{TotalQuestions}:\n{_currentTFQuizQuestion.tfQuestions}\n" +
                       $"\n{_currentTFQuizQuestion.tfAnswers}\nPlease answer with A (True) or B (False):";
            }
        }

        private static int ConvertAnswerToIndex(string input)
        {
            input = input.Trim().ToUpper();

            switch (input)
            {
                case "A":
                    return 0;
                case "B":
                    return 1;
                case "C":
                    return 2;
                case "D":
                    return 3;
                default:
                    return -1; // Invalid input
            }
        }

        private static int ConvertTFAnswerToIndex(string input)
        {
            input = input.Trim().ToUpper();

            if (input == "A") return 0; // True
            if (input == "B") return 1; // False

            return -1; // Invalid input
        }

        private static string IndexToLetter(int index)
        {
            switch (index)
            {
                case 0: return "A";
                case 1: return "B";
                case 2: return "C";
                case 3: return "D";
                default: return "?";
            }
        }

    }
}

//------------------------------------------------------------------------------------------------------------------------//

//-------------------------------------------------END OF FILE-------------------------------------------------//

