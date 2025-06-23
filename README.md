# 📝 SimpleBlog - A Dynamic Blog Platform with Real-time Notifications

SimpleBlog has evolved beyond a basic content-sharing site into a feature-rich, interactive, and modern web application. It's a complete content management system built from the ground up, featuring a powerful role-based user system, a dynamic user interface, and a sophisticated real-time notification system powered by SignalR.

---

## ✨ Key Features

This platform is packed with features designed for a rich user experience and robust administrative control.

### 🏡 Dynamic User Experience
- **Interactive Homepage:** A dynamic "magazine-style" landing page featuring a "Hero" section for the latest post, a swipeable carousel for recent articles (powered by Swiper.js), and a section for popular categories.
- **Intelligent Search:** A robust search functionality integrated directly into the main blog page, supporting live filtering and full pagination of search results.
- **Rich Text Editor:** A WYSIWYG editor (TinyMCE) is integrated for an intuitive post-writing experience, allowing for easy formatting like bold, italics, lists, and headings.
- **🌍 Multi-Language Post Support:** The blog is designed to handle both Arabic (RTL) and English (LTR) content seamlessly.

### 🚀 Real-time Notification System (SignalR)
A complete, real-time notification system to keep users engaged.
- **In-App Bell Icon:** A notification bell in the navbar alerts users to new activity with a dynamic counter for unread notifications.
- **Real-time Toast Popups:** Instant, non-intrusive toast popups appear when a user receives a new notification.
- **Interactive Dropdown with Infinite Scroll:** The notification list features **Infinite Scroll**, allowing users to browse their entire notification history seamlessly.
- **Smart "Mark as Read" & Data Integrity:** Notifications are marked as read upon click, and deleting a comment automatically removes its associated notification, keeping the system clean and up-to-date in real-time.

### 🔐 Authentication & Authorization (ASP.NET Core Identity)
- **Role-Based Access Control (RBAC):** Distinct roles (`Admin`, `Author`, `User`) with granular permissions.
- **Secure CRUD Operations:** Users can only edit or delete their own content, while Admins have full override privileges.
- **Custom User Profiles:** Users have a `DisplayName` and a `ProfileImagePath`.
- **Admin Control Panel:** A secure dashboard for admins to manage user accounts and assign roles.

### 👨‍💻 Core Blog Functionality
- **Full Post & Comment Management:** Full CRUD (Create, Read, Update, Delete) capabilities.
- **Image Uploads:** Easily upload and associate a main image with each post.
- **Category System:** Admins can create and manage categories for content organization.

---

## 🚀 Technologies Used

- **Backend:**
    - **.NET 8** & **ASP.NET Core MVC**
    - **Entity Framework Core 8** for data access.
    - **ASP.NET Core Identity** for user management.
    - **SignalR** for real-time web functionality.
- **Database:**
    - **SQLite** (for development flexibility).
- **Frontend:**
    - **Razor Views** & **Bootstrap 4**
    - **JavaScript (ES6)** & **jQuery** for client-side interactivity.
    - **Swiper.js** for carousels.
    - **TinyMCE** for the rich text editor.

---

## 🏁 Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or a code editor of your choice.

### Installation
1.  **Clone the repo:**
    ```sh
    git clone [https://github.com/YoussefNour1/SimpleBlog.git](https://github.com/YoussefNour1/SimpleBlog.git)
    ```
2.  **Navigate to the project directory:**
    ```sh
    cd SimpleBlog
    ```
3.  **Restore NuGet packages:**
    ```sh
    dotnet restore
    ```
4.  **Apply database migrations:**
    This command will create the `SimpleBlog.db` SQLite database file with the complete schema.
    ```sh
    dotnet ef database update
    ```
5.  **Run the application:**
    ```sh
    dotnet run
    ```
The application will be accessible at `https://localhost:port` and `http://localhost:port`, where `port` is specified in the console output.

### 🔑 Admin Credentials
The application automatically seeds the database with roles ("Admin", "Author").
- **Admin Email:** `youssef@gmail.com`
- **The password:** Contact me through my email `youssef.nour1612@gmail.com` so I give it to you.
