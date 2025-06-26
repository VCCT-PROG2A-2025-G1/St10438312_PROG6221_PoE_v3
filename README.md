Cybersecurity Awareness Chatbot

Features
Enhanced Chat Experience
Personalized Conversation: Greets users by name and remembers their favorite cybersecurity topic

Sentiment Detection: Recognizes emotional keywords (e.g., "worried", "curious", "frustrated") and tailors responses accordingly

Conversation Summaries: Provides detailed summaries when requested (summary, recap, overview)

Task Management System
Create Tasks: Add reminders with titles, descriptions, and due dates

Track Progress: Mark tasks as complete or delete them

Task Overview: View all tasks by asking "show my tasks" in chat

Activity Logging
Comprehensive Tracking: Logs tasks, quiz attempts, and chat topics

Security Topic Detection: Highlights discussions about phishing, passwords, scams, and safe browsing


Quiz System
Multiple Quiz Types: Choose between multi-choice and true/false type quiz


User Interface
Modern WPF Interface: Clean, dark-themed desktop application

Multiple Views: Switch between chatbot, help, quiz, and activity log

Setup Instructions

Prerequisites
Visual Studio 2022 (or later)
.NET Framework 4.8
Windows 10/11


Usage Instructions
Starting the Chatbot
The bot will greet you with a welcome message

The interface has three main sections accessible via the left sidebar:

Chatbot: Main conversation interface

Quiz: Take cybersecurity quizzes

Activity Log: View your conversation history and activities

Chat Features
Ask cybersecurity questions:

EXAMPLE:
What is phishing?
How to create strong passwords?
What are common online scams?
Request conversation summaries:

EXAMPLE:
Can you summarize our chat?
Give me a recap
What have we discussed?
Manage tasks through chat:

EXAMPLE:
Add task: Call IT about password policy
Show my tasks
What reminders do I have?
Add Tasks:

Enter title, description, and set reminder date/time

Click "Add Task" or press Enter

Complete Tasks:

Click the green checkmark ✓ to mark as complete

Delete Tasks:

Click the red X to remove tasks

Quiz Features
Select quiz type (Multi-Choice or True/False)

Answer questions and view immediate feedback

Example Conversations
Chat Example
text
Bot: Hello! I'm your Cybersecurity Assistant. What's your name?
User: Alex

Bot: Welcome, Alex! Which security topic interests you most? 
(phishing/passwords/scams/safe browsing)

User: phishing

Bot: I'll remember phishing is your focus area. Ask me anything!

User: I'm worried about email scams
Bot: I understand your concern, Alex. For phishing emails, always check...

User: summary
Bot: Conversation Summary
     Total messages: 6
     Topics discussed: phishing, email scams
     1 pending task: Review email security (due Friday)
     
Task Management Example
User: Add task: Change work password every 90 days

Bot: Added task! Due date set to 90 days from now.

User: show my tasks
Bot: Your Tasks:
     - Change work password (due Mar 15)
     - Complete security training (due Feb 28)
     
Supported Commands
help - Show available commands
summary - Get conversation recap


Task Commands
add task [description] - Create new reminder
show tasks - List all tasks
complete task [number] - Mark task as done

Quiz Commands
start quiz - Begin a new quiz

Technical Details
MVVM Pattern: Separates UI from business logic
WPF Framework: Modern Windows desktop interface
Modular Design: Independent components for chat, tasks, and quizzes

Data Tracking
Conversation history stored in memory

Task data persists during session

Quiz results saved for performance tracking
