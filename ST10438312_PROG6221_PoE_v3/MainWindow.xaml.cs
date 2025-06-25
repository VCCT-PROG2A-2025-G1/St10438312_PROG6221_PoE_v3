using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        // Existing chatbot variables
        private List<ChatMessage> messages = new List<ChatMessage>();
        private readonly ChatBot_Response _chatBotResponse;
        private readonly CybersecurityQuiz _cybersecurityQuiz;
        private ChatBot _chatBot = new ChatBot(new ChatBot_Response(), new CybersecurityQuiz());

        // Task management system
        private ObservableCollection<TaskItem> _tasks = new ObservableCollection<TaskItem>();
        private int _taskIdCounter = 1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimePicker();
            HandleNewMessage(_chatBot.StartUpMessage(), false);
            SendButton.Click += SendButton_Click;
            TasksItemsControl.ItemsSource = _tasks;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ChatbotRadio.IsChecked = true;
            RadioButton_Checked(ChatbotRadio, null);
        }

        private void InitializeTimePicker()
        {
            // Clear any existing items
            TaskReminderTimePicker.Items.Clear();

            // Add time slots in 30-minute intervals
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    DateTime time = DateTime.Today.AddHours(hour).AddMinutes(minute);
                    TaskReminderTimePicker.Items.Add(time.ToString("h:mm tt"));
                }
            }

            // Set default to current time rounded to nearest 30 minutes
            DateTime now = DateTime.Now;
            int defaultMinute = now.Minute < 30 ? 0 : 30;
            DateTime defaultTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, defaultMinute, 0);
            TaskReminderTimePicker.Text = defaultTime.ToString("h:mm tt");
        }

        // Task model class
        public class TaskItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Reminder { get; set; }
            public bool IsCompleted { get; set; }

            // Formatted properties for display
            public string ReminderDate => Reminder.ToString("d");
            public string ReminderTime => Reminder.ToString("t");
            public string TimeRemaining
            {
                get
                {
                    TimeSpan remaining = Reminder - DateTime.Now;
                    if (remaining.TotalDays >= 1)
                        return $"Due in {Math.Floor(remaining.TotalDays)} days";
                    if (remaining.TotalHours >= 1)
                        return $"Due in {Math.Floor(remaining.TotalHours)} hours";
                    return "Due soon";
                }
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Parse the selected time
                DateTime reminderDateTime;

                if (TaskReminderDatePicker.SelectedDate.HasValue)
                {
                    DateTime selectedDate = TaskReminderDatePicker.SelectedDate.Value;
                    string timeString = TaskReminderTimePicker.Text;

                    if (DateTime.TryParse(timeString, out DateTime time))
                    {
                        reminderDateTime = selectedDate.Date.Add(time.TimeOfDay);
                    }
                    else
                    {
                        reminderDateTime = selectedDate.Date.AddHours(12); // Default to noon if time parsing fails
                    }
                }
                else
                {
                    reminderDateTime = DateTime.Now.AddDays(1).Date.AddHours(12); // Default to tomorrow noon
                }

                var newTask = new TaskItem
                {
                    Id = _taskIdCounter++,
                    Title = TaskTitleTextBox.Text,
                    Description = TaskDescriptionTextBox.Text,
                    Reminder = reminderDateTime,
                    IsCompleted = false
                };

                _tasks.Add(newTask);
                ClearTaskInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearTaskInputFields()
        {
            TaskTitleTextBox.Text = "";
            TaskDescriptionTextBox.Text = "";
            TaskReminderDatePicker.SelectedDate = null;
            TaskTitleTextBox.Focus();
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int taskId)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId);
                if (task != null)
                {
                    task.IsCompleted = true;
                    _tasks.Remove(task);
                }
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int taskId)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId);
                if (task != null)
                {
                    if (MessageBox.Show($"Delete '{task.Title}'?", "Confirm",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        _tasks.Remove(task);
                    }
                }
            }
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
                MaxWidth = 400,
                Margin = new Thickness(50, 5, 10, 5),
                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 16,
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
                MaxWidth = 400,
                Margin = new Thickness(10, 5, 50, 5),
                Child = new TextBlock
                {
                    Text = message,
                    FontSize = 16,
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            {
                if (!(sender is RadioButton radioButton)) return;

                // Safely hide all views if they exist
                ChatScrollViewer?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                QuizView?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                ChatLogView?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                HelpView?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
                InputArea?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);

                // Show the appropriate view
                switch (radioButton.Name)
                {
                    case "ChatbotRadio":
                        ChatScrollViewer?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                        InputArea?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                        if (ViewTitle != null) ViewTitle.Content = "Chatbot";
                        break;

                    case "QuizRadio":
                        QuizView?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                        if (ViewTitle != null) ViewTitle.Content = "Quiz";
                        break;

                    case "ChatLogRadio":
                        ChatLogView?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                        if (ViewTitle != null) ViewTitle.Content = "Chat Log";
                        break;

                    case "HelpRadio":
                        HelpView?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                        if (ViewTitle != null) ViewTitle.Content = "Help";
                        break;
                }
            }
        }

    }
}
