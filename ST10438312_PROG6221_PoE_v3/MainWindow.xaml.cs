using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ST10438312_PROG6221_PoE_v3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<ChatMessage> messages = new List<ChatMessage>();
        private readonly ChatBot_Response _chatBotResponse;
        private ChatBot _chatBot = new ChatBot(new ChatBot_Response());


        //----------------------------------------------------------------------------------//
        public MainWindow()
        {
            InitializeComponent();
            HandleNewMessage(_chatBot.StartUpMessage(), false);
            SendButton.Click += SendButton_Click;
        }
        //----------------------------------------------------------------------------------//

        //----------------------------------------------------------------------------------//
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        //----------------------------------------------------------------------------------//


        //----------------------------------------------------------------------------------//
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        //----------------------------------------------------------------------------------//


        //----------------------------------------------------------------------------------//
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //----------------------------------------------------------------------------------//




        //----------------------------------------------------------------------------------//
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {

            string userInput = MessageTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            HandleNewMessage(userInput, true);
            MessageTextBox.Text = "";

            string botResponse = _chatBot.ProcessUserInput(userInput);
            HandleNewMessage(botResponse, false);
        }
        //----------------------------------------------------------------------------------//


        //----------------------------------------------------------------------------------//
        public void HandleNewMessage(string messageText, bool isUser)
        {
            // Store the message data
            var chatMessage = new ChatMessage(messageText, isUser);
            messages.Add(chatMessage);

            // Display it visually
            if (isUser)
            {
                AddUserMessage(chatMessage.Message);
            }
            else
            {
                AddBotMessage(chatMessage.Message);
            }
        }
        //----------------------------------------------------------------------------------//


        //----------------------------------------------------------------------------------//
        public void AddUserMessage(string message)
        {
            var userBubble = new Border
            {

                Background = new SolidColorBrush(Color.FromRgb(88, 101, 242)),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth = 300,
                Margin = new Thickness(50, 5, 10, 5),
                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            ChatPanel.Children.Add(userBubble);
            ChatScrollViewer.ScrollToEnd();
        }
        //----------------------------------------------------------------------------------//


        //----------------------------------------------------------------------------------//
        public void AddBotMessage(string message)
        {
            var botBubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(64, 68, 75)),
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 350,
                Margin = new Thickness(10, 5, 50, 5),
                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 12,
                    Foreground = Brushes.White,
                    TextWrapping = TextWrapping.Wrap
                }
            };

            ChatPanel.Children.Add(botBubble);
            ChatScrollViewer.ScrollToEnd();
        }
        //----------------------------------------------------------------------------------//

        public Action<string> GetMessageHandler(string userInput)
        {
            return AddBotMessage;
        }
    }
}
