# 🤖 ChattyBot

## 📋 Description

**ChattyBot** is an interactive chat application featuring a built-in virtual assistant designed to make messaging fun, engaging, and productive. Instead of just sending plain text, users can interact with the bot using over 15 custom commands to play mini-games, solve math equations, encrypt secret messages, or test their knowledge with multi-category trivia. 

The application provides a complete workspace where you can easily manage your chat history. Users can securely log in, create multiple distinct conversations, rename or delete them, and even export their favorite chats to local files. Whether you want a quick game of Rock Paper Scissors, a random song recommendation, or just a good joke to brighten your day, ChattyBot handles it all directly within a single, seamless chat interface.<br><br>

## ⭐ Key Features
* **`User Authentication`**: Secure account registration and login system powered by JWT tokens for safe session management.
* **`Conversation Management`**: Create multiple distinct chat threads, rename active conversations, and delete old ones to keep your workspace organized.
* **`Interactive Bot Engine`**: Access over 15 dynamic commands directly within the chat to perform math calculations, encrypt text (Caesar Cipher), or translate messages to Morse code.
* **`Mini-Games & Trivia`**: Challenge the bot to a Dice Duel, play Rock Paper Scissors, or start a multi-category trivia game.
* **`Entertainment & Recommendations`**: Instantly request random jokes, fun facts, memes, inspirational quotes, or music and video game recommendations based on specific genres.
* **`Data Portability`**: Easily download and back up your complete chat histories and bot interactions to your local device in structured JSON or XML formats.<br><br>

## 🛠️ Tech Stack
| **Layer** | **Technologies** |
| :---: | :---: |
| **Backend & API** | C# (.NET 10), Entity Framework Core, ASP.NET Core Web API |
| **Frontend** | Blazor WebAssembly, HTML, CSS, Bootstrap Icons |
| **Database** | MySQL |
| **Testing** | xUnit, bUnit, NSubstitute, FluentAssertions |
| **Version Control** | Git |


<br><br>

## 🏗️ Architecture

**ChattyBot** is built on a decoupled **`Client-Server`** architecture, communicating exclusively through a RESTful API. This design ensures a clear separation of concerns, high maintainability, and a fast, fluid user experience.

* **Backend (ASP.NET Core):** Implements a strict **Layered Architecture** divided into four logical levels:
  * **API Layer:** Acts as the entry point, handling incoming HTTP requests and routing.
  * **Application (Core) Layer:** The "brain" of the system, using the *Command Pattern* to execute bot commands and process business logic dynamically.
  * **Infrastructure Layer:** Abstracts data access and database operations using repositories.
  * **Data Storage Layer:** The physical MySQL database where users, sessions, and chat histories are permanently stored.

* **Frontend (Blazor WebAssembly):** A **Single Page Application (SPA)** built on a **Component-Based Architecture**. The user interface is divided into modular, reusable components that manage state locally. This allows for instant UI updates and seamless navigation without requiring full page reloads from the server.<br><br>

## 📂 Project Structure
```text
ChattyBot/
├── .github/                    
├── screenshots/                  # Application preview images for documentation
├── src/                          # Source code
│   └── ChattyBot/                
│       ├── ChattyBot.Client/     # Frontend: Blazor WebAssembly SPA
│       │   ├── Components/       # Reusable UI components
│       │   ├── Layout/           # Main layout and navigation components
│       │   ├── Pages/            # Routable views (Home, Login, Chats, ManageAccount)
│       │   ├── Properties/       # Project properties and launch settings
│       │   ├── Services/         # External API client callers and Auth State
│       │   └── wwwroot/          # Static web assets (CSS, images, icons)
│       │
│       ├── ChattyBot.Server/     # Backend: ASP.NET Core Web API
│       │   ├── API/Controllers/  # Entry points for HTTP requests routing
│       │   ├── Application/      # Core Business Logic (Bot Engine, Commands, Services)
│       │   ├── Domain/           # Core models (Entities and Enums)
│       │   ├── Infrastructure/   # EF Core DbContext, Repositories, and JWT Security
│       │   └── Properties/       # Backend launch settings and environment configs
│       │
│       ├── ChattyBot.Shared/     # Shared data contracts across client and server
│       │   └── Contracts/        # Shared cross-project data structures
│       │       ├── DTO/          # Data Transfer Objects (LoginDTO, CreateChatDTO, etc.)
│       │       └── Enums/        # Shared enumerations (MessageType)
│       │
│       ├── ChattyBot.Tests/      # Comprehensive test suite
│       │   ├── Client/           # UI Component tests using bUnit
│       │   ├── Integration/      # End-to-end API tests (WebApplicationFactory)
│       │   └── Server/           # Unit tests for bot logic, services, and utils (xUnit)
│       │
│       └── ChattyBot.slnx        # Main .NET Solution file
│
├── tests/                        
├── .gitignore
├── LICENSE
└── README.md                     # Project documentation
```

<br>

## 🚀 Setup and Installation

1. **Prerequisites:** 
Before running the project locally, ensure you have the following installed on your system:
* **.NET 10 SDK** to compile and run the application.
* **MySQL Server** instance (local, XAMPP, or a Docker container) to host the database.
* **Visual Studio 2026** (or JetBrains Rider / VS Code) with the ASP.NET and web development workload.

2. **Installation:**

* **Clone the repository:**
	```bash
	git clone https://github.com/AlexMules/ChattyBot.git
	cd ChattyBot/src/ChattyBot
	```
* **Configure the Database Connection:**
	Navigate to the server project (**`ChattyBot.Server`**) and open **`appsettings.json`** or **`appsettings.Development.json`**. 
	Update the connection string with your MySQL server credentials:
	```bash
	"ConnectionStrings": {
		"DefaultConnection": "Server=localhost;Port=3306;Database=ChattyBotDb;Uid=your_mysql_user;Pwd=your_mysql_password;"
	}
 	```
	
* **Apply EF Core Migrations:**
	Open a terminal in the root solution directory and run the following command to initialize the MySQL 
	database schema:
	```bash
	dotnet ef database update --project ChattyBot.Server
	```
* **Run the project:**
	Launch the solution via Visual Studio 2026 by configuring it to start both the server project 
	and the Blazor WebAssembly client. Alternatively, execute it from the terminal:
	```bash
	dotnet run --project ChattyBot.Server
	dotnet run --project ChattyBot.Client
	```
* Open your browser and navigate to the local URL provided in the terminal output 
   (https://localhost:7296 or http://localhost:5075).<br><br>

## 📖 User Guide

Once the application is up and running in your browser, follow these steps to experience its core functionalities:

### 1. Account Creation and Authentication
* Click on **Register** to create a new user account by providing an email, username, and password.
* Log in with your new credentials. You will be redirected to the main workspace.

### 2. Starting a Conversation
* Click on the **Chats** section.
* In the left sidebar, click the **New Chat** button.
* Enter a title for your conversation in the modal window and click **Create**.
* Your newly created conversation will appear at the top of the **Recent Conversations** list and open automatically.

### 3. Interacting with the Chatbot
Type your text in the bottom chat input area. To interact with the bot's dynamic engine, use any of the supported command patterns:
* Type `/help` to see the full layout of available modules.
* Use calculation rules like `/calc 25 * 4 + (10 / 2)` to get instant math evaluations.
* Launch interactive features such as `/trivia -gaming` or `/dice-duel`. Note that during trivia challenges, the message input box will lock down natively, prompting you to pick an answer choice directly from the specialized UI bubble to unlock the chat.

### 4. Managing Conversations and Data Export
* Hover over any conversation item in the sidebar list and click the **Three-Dots Menu** icon.
* Click **Rename** to update the chat's title using the modal overlay, or click **Delete** to wipe the conversation and its underlying history from the database completely.
* Click the **Export** icon located in the upper-right area of the chat header to safely download a local copy of your current chat history in either **JSON** or **XML** file structures.

<br><br>

## 🖼️ Screenshots

![RegisterPage](./screenshots/RegisterPage.jpg)<br><br>
![LoginPage](./screenshots/LoginPage.jpg)<br><br>
![ProfilePage](./screenshots/ProfilePage.jpg)<br><br>
![ManageAccountPage](./screenshots/ManageAccountPage.jpg)<br><br>
![ChatsPage1](./screenshots/ChatsPage(1).jpg)<br><br>
![ChatsPage2](./screenshots/ChatsPage(2).jpg)<br><br>
![ChatsPage3](./screenshots/ChatsPage(3).jpg)<br><br>
![HelpCommand](./screenshots/HelpCommand.jpg)<br><br>
![AboutCommand](./screenshots/AboutCommand.jpg)<br><br>
![CalcCommand](./screenshots/CalcCommand.jpg)<br><br>
![ChooseCommand](./screenshots/ChooseCommand.jpg)<br><br>
![CoinFlipCommand](./screenshots/CoinFlipCommand.jpg)<br><br>
![DiceCommand](./screenshots/DiceCommand.jpg)<br><br>
![DiceDuelCommand](./screenshots/DiceDuelCommand.jpg)<br><br>
![EncryptCommand](./screenshots/EncryptCommand.jpg)<br><br>
![FunFactCommand](./screenshots/FunFactCommand.jpg)<br><br>
![JokeCommand](./screenshots/JokeCommand.jpg)<br><br>
![MemeCommand](./screenshots/MemeCommand.jpg)<br><br>
![MorseCommand](./screenshots/MorseCommand.jpg)<br><br>
![MusicCommand](./screenshots/MusicCommand.jpg)<br><br>
![QuoteCommand](./screenshots/QuoteCommand.jpg)<br><br>
![RandomCommand](./screenshots/RandomCommand.jpg)<br><br>
![ReverseCommand](./screenshots/ReverseCommand.jpg)<br><br>
![RpsCommand](./screenshots/RpsCommand.jpg)<br><br>
![TriviaCommand](./screenshots/TriviaCommand.jpg)<br><br>
![VideoGameCommand](./screenshots/VideoGameCommand.jpg)
