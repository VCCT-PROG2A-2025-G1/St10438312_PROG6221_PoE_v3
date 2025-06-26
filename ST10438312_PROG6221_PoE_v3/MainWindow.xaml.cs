// Andre Fourie
// ST10438312
// Group 1

// Refrences 
/*    
 *    https://learn.microsoft.com/en-us/dotnet/csharp/
 *    https://www.w3schools.com/cs/index.php
 *    https://www.youtube.com/watch?v=PzP8mw7JUzI&t=626s
 *    https://www.youtube.com/watch?v=QuczbW66ejw&t=109s
 *    https://www.youtube.com/watch?v=rfMIayISSW0&t=8s
 *    https://www.youtube.com/watch?v=V9DkvcT27WI&t=558s
 *    https://www.youtube.com/watch?v=mIY8TRPNx1o&t=187s
 *    https://www.youtube.com/watch?v=oSeYvMEH7jc
 *    https://learn.microsoft.com/en-us/windows/apps/desktop/
 *    https://learn.microsoft.com/en-us/windows/apps/design/layout/layouts-with-xaml
 *    https://www.tutorialspoint.com/wpf/wpf_xaml_overview.htm
 *    https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.datetimepicker?view=windowsdesktop-9.0
 */

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
        #region Fields and Properties
        // Chatbot components
        private List<ChatMessage> messages = new List<ChatMessage>();
        private readonly ChatBot_Response _chatBotResponse;
        private readonly CybersecurityQuiz _cybersecurityQuiz;
        private ChatBot _chatBot = new ChatBot(new ChatBot_Response(), new CybersecurityQuiz());

        // Task management
        private ObservableCollection<TaskItem> _tasks = new ObservableCollection<TaskItem>();
        private int _taskIdCounter = 1;

        // Activity log
        private ObservableCollection<ActivityLog> _activityLogs = new ObservableCollection<ActivityLog>();
        private const int LogBatchSize = 5;
        private int _currentLogDisplayCount = LogBatchSize;
        private List<string> _securityKeywords = new List<string> { "phishing", "password", "passwords", "scam", "safe browsing" };
        #endregion

        #region Constructor and Initialization
        //-------------------------------------------------------------------------------------//
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            InitializeChatLog();


            TasksItemsControl.ItemsSource = _tasks;
        
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTimePicker(); // Now called after all controls are loaded
            InitializeChatLog();
            HandleNewMessage(_chatBot.StartUpMessage(), false);

            if (ViewTitle != null)
                ViewTitle.Content = "Chatbot";

            ChatbotRadio.IsChecked = true;
            RadioButton_Checked(ChatbotRadio, null);
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private void InitializeTimePicker()
        {
            TaskReminderTimePicker.Items.Clear();
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute < 60; minute += 30)
                {
                    DateTime time = DateTime.Today.AddHours(hour).AddMinutes(minute);
                    TaskReminderTimePicker.Items.Add(time.ToString("h:mm tt"));
                }
            }
            DateTime now = DateTime.Now;
            int defaultMinute = now.Minute < 30 ? 0 : 30;
            DateTime defaultTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, defaultMinute, 0);
            TaskReminderTimePicker.Text = defaultTime.ToString("h:mm tt");
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private void InitializeChatLog()
        {
            ActivityLogItemsControl.ItemsSource = new ListCollectionView(_activityLogs)
            {
                Filter = o => _activityLogs.IndexOf((ActivityLog)o) >= (_activityLogs.Count - _currentLogDisplayCount)
            };
        }
        //-------------------------------------------------------------------------------------//
        #endregion

        #region Window Management
        //-------------------------------------------------------------------------------------//
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Custom button to minimize window
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Custom Button to exit window
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //-------------------------------------------------------------------------------------//
        #endregion

        #region View Navigation
        //-------------------------------------------------------------------------------------//
        // Method to check whch radio button has been pressed
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
            if (!(sender is RadioButton radioButton)) return;
            // Makes it so that the windows are not visible yet
            ChatScrollViewer?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            ChatLogView?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            TaskView?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            InputArea?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);

            // Checks to see what radioButton has been pressed, 
            // Button that corresponds with window will become visible when clicked
            switch (radioButton.Name)
            {
                // Makes the Chatbot visible
                case "ChatbotRadio":
                    ChatScrollViewer?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                    InputArea?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                    // IF statement that ensure chatbot window is the default
                    if (ViewTitle != null) 
                        ViewTitle.Content = "Chatbot";
                    break;

                // Makes the Chat Log Visible
                case "ChatLogRadio":
                    ChatLogView?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                    ViewTitle.Content = "Activity Log";
                    RefreshActivityLog();
                    ShowMoreLogsButton.Visibility =
                        _activityLogs.Count > LogBatchSize ? Visibility.Visible : Visibility.Collapsed;
                    break;

                // Makes the task view visible
                case "HelpRadio":
                    TaskView?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
                    ViewTitle.Content = "Tasks & Reminders";
                    break;
            }
        }
        //-------------------------------------------------------------------------------------//
        #endregion

        #region Task Management
        //-------------------------------------------------------------------------------------//
        // Method for adding a task
        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Checks to see if something has been entered in the taskbox
            if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            {
                MessageBox.Show("Please enter a task title", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Gets the date and time the user selected
                DateTime reminderDateTime = TaskReminderDatePicker.SelectedDate.HasValue
                    ? ParseReminderDateTime(TaskReminderDatePicker.SelectedDate.Value, TaskReminderTimePicker.Text)
                    : DateTime.Now.AddDays(1).Date.AddHours(12);

                // Adds new task, fills in all the values of new task
                var newTask = new TaskItem
                {
                    Id = _taskIdCounter++,
                    Title = TaskTitleTextBox.Text,
                    Description = TaskDescriptionTextBox.Text,
                    Reminder = reminderDateTime,
                    IsCompleted = false
                };

                // Adds the new task to the log
                _tasks.Add(newTask);
                LogTaskActivity("Task Created", newTask);
                ClearTaskInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private DateTime ParseReminderDateTime(DateTime date, string timeString)
        {
            if (DateTime.TryParse(timeString, out DateTime time))
            {
                return date.Date.Add(time.TimeOfDay);
            }
            return date.Date.AddHours(12);
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Method that clears areas after user adds task
        private void ClearTaskInputFields()
        {
            TaskTitleTextBox.Text = "";
            TaskDescriptionTextBox.Text = "";
            TaskReminderDatePicker.SelectedDate = null;
            TaskTitleTextBox.Focus();
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Method to set task as complete
        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            // Checks to see if user pressed button and what task to delete
            if (sender is Button button && button.Tag is int taskId)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId);
                if (task != null)
                {
                    task.IsCompleted = true;
                    task.CompletedDate = DateTime.Now;
                    LogTaskActivity("Task Completed", task);
                }
            }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Method to delete the task from the list
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int taskId)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == taskId);

                // If statement to confirm if the user want to delete task
                if (task != null && MessageBox.Show($"Delete '{task.Title}'?", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Removes the tasks
                    _tasks.Remove(task);
                    LogTaskActivity("Task Deleted", task);
                }
            }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Method to add task to the log
        private void LogTaskActivity(string action, TaskItem task)
        {
            _activityLogs.Add(new ActivityLog
            {
                Timestamp = DateTime.Now,
                ActivityType = "Task",
                Description = $"{action}: {task.Title}",
                Details = new Dictionary<string, string>
            {
                {"Status", task.IsCompleted ? "Completed" : "Pending"},
                {"Due Date", task.Reminder.ToString("g")}
            }
            });
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Checks to see if the user wants to view tasks
        private string HandleTaskQuery(string userInput)
        {
            userInput = userInput.ToLower();

            // Checks if user input contains these words
            if (userInput.Contains("reminder") || userInput.Contains("task") || userInput.Contains("todo"))
            {
                return GetTaskSummary();
            }
            return null;
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Method to get the task summary
        private string GetTaskSummary()
        {
            // Checks if a task has been added
            if (_tasks.Count == 0) return "You don't have any tasks yet. Would you like to add one?";

            // Gets the active tasks
            var activeTasks = _tasks.Where
                (t => !t.IsCompleted).OrderBy(t => t.Reminder).ToList();

            // Get complete tasks
            var completedTasks = _tasks.Where
                (t => t.IsCompleted).OrderByDescending(t => t.CompletedDate).ToList();


            // Create the output for the task summary in chatbot
            StringBuilder response = new StringBuilder();
            if (activeTasks.Any())
            {
                response.AppendLine("---Your Active Tasks---");
                foreach (var task in activeTasks)
                {
                    response.AppendLine($"- {task.Title} (Due: {task.Reminder:MMM dd, h:mm tt})");
                    if (!string.IsNullOrEmpty(task.Description))
                    {
                        response.AppendLine($"  *{task.Description}*");
                    }
                }
            }

            if (completedTasks.Any())
            {
                response.AppendLine("\n---Completed Tasks---");
                foreach (var task in completedTasks.Take(5))
                {
                    response.AppendLine($"- {task.Title} (Completed: {task.CompletedDate:MMM dd})");
                }
            }

            response.AppendLine($"\nYou have {activeTasks.Count} active and {completedTasks.Count} completed tasks.");
            return response.ToString();
        }
        //-------------------------------------------------------------------------------------//
        #endregion

        #region Chat Functionality
        //-------------------------------------------------------------------------------------//
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = MessageTextBox.Text.Trim();

            if (string.IsNullOrEmpty(userInput)) return;

            HandleNewMessage(userInput, true);
            MessageTextBox.Text = "";

            // Check for summary request
            if (userInput.ToLower().Contains("summary") || userInput.ToLower().Contains("recap") || userInput.ToLower().Contains("overview"))
            {
                string summary = GetChatSummary();
                HandleNewMessage(summary, false);
                return;
            }

            // Check if it's a task-related query
            string taskResponse = HandleTaskQuery(userInput);
            if (taskResponse != null)
            {
                HandleNewMessage(taskResponse, false);
                return;
            }

            // Otherwise process as normal chatbot message
            string botResponse = _chatBot.ProcessUserInput(userInput);
            HandleNewMessage(botResponse, false);
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        public void HandleNewMessage(string messageText, bool isUser)
        {
            var chatMessage = new ChatMessage(messageText, isUser);
            messages.Add(chatMessage);

            // Log bot messages for activity tracking
            if (!isUser){ LogChatActivity (messageText);}

            // Display the message in the appropriate format
            if (isUser){
                AddUserMessage (chatMessage.Message); }

            else {
                AddBotMessage (chatMessage.Message); }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private void LogChatActivity(string message)
        {
            var foundKeywords = _securityKeywords
                .Where(k => message.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (foundKeywords.Any())
            {
                _activityLogs.Add(new ActivityLog
                {
                    Timestamp = DateTime.Now,
                    ActivityType = "Chat",
                    Description = "Cybersecurity topic discussed",
                    Details = new Dictionary<string, string>
                {
                    {"Keywords", string.Join(", ", foundKeywords)},
                    {"Topic", GetSecurityTopic(foundKeywords)},
                    {"Summary", GetTopicSummary(foundKeywords)}
                }
                });
            }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private string GetChatSummary()
        {
            if (messages.Count == 0)
            {
                return "There's no conversation history yet.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("---Conversation Summary---");
            sb.AppendLine($"Total messages: {messages.Count}");

            // Count user vs bot messages
            int userMessages = messages.Count(m => m.IsUser);
            int botMessages = messages.Count - userMessages;
            sb.AppendLine($"- Total User Messages: {userMessages} ");
            sb.AppendLine($"- Total Bot Messages: {botMessages} ");

            // Get recent topics
            var recentKeywords = _activityLogs
                .Where(log => log.ActivityType == "Chat")
                .SelectMany(log => log.Details["Keywords"].Split(','))
                .Select(k => k.Trim())
                .Distinct()
                .Take(3)
                .ToList();

            if (recentKeywords.Any())
            {
                sb.AppendLine("\nRecent topics discussed:");
                foreach (var keyword in recentKeywords)
                {
                    sb.AppendLine($"- {keyword}");
                }
            }

            // List pending tasks
            int activeTasks = _tasks.Count(t => !t.IsCompleted);
            if (activeTasks > 0)
            {
                sb.AppendLine($"\nYou have {activeTasks} pending tasks:");
                foreach (var task in _tasks.Where(t => !t.IsCompleted).Take(3))
                {
                    sb.AppendLine($"- {task.Title} (due {task.Reminder:MMM dd})");
                }
            }

            return sb.ToString();
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Adds a new user message bubble to the chat panel with appropriate styling.
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
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        //  Adds a new bot message bubble to the chat panel with appropriate styling.
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
        //-------------------------------------------------------------------------------------//
        #endregion


        #region Activity Log
        //-------------------------------------------------------------------------------------//
        private void RefreshActivityLog()
        {
            ((ListCollectionView)ActivityLogItemsControl.ItemsSource).Refresh();
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        private void ShowMoreLogsButton_Click(object sender, RoutedEventArgs e)
        {
            _currentLogDisplayCount += LogBatchSize;
            RefreshActivityLog();

            if (_currentLogDisplayCount >= _activityLogs.Count)
            {
                ShowMoreLogsButton.Visibility = Visibility.Collapsed;
            }
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Method to get keywords in userInput
        private string GetSecurityTopic(List<string> keywords)
        {
            if (keywords.Contains("phishing"))
                return "Phishing Awareness";

            if (keywords.Contains("password")) 
                return "Password Security";

            if (keywords.Contains("scam")) 
                return "Online Scams";

            if (keywords.Contains("safe browsing"))
                return "Safe Browsing";

            return "Security Discussion";
        }
        //-------------------------------------------------------------------------------------//

        //-------------------------------------------------------------------------------------//
        // Stores how the keywords will be displayed in the task view
        private string GetTopicSummary(List<string> keywords)
        {
            // Dictionary for the keys and definitions
            var summaries = new Dictionary<string, string>
        {
            {"phishing", "How to identify and avoid phishing attempts"},
            {"password", "How to create secure password"},
            {"scam", "How to recognize and prevennt scams"},
            {"safe browsing", "How to safely browse the internet"}
        };

            return string.Join("; ", keywords
                .Where(k => summaries.ContainsKey(k))
                .Select(k => summaries[k])
                .Distinct());
        }
        //-------------------------------------------------------------------------------------//
        #endregion
    }
}
