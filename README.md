# 📝 SimpleBlog - A Feature-Rich Blog Platform

SimpleBlog is a robust and secure blog platform built with modern web technologies. It's more than just a place to write posts; it's a complete content management system with a powerful role-based user management system.

---

## ✨ Key Features

### 👨‍💻 Core Blog Functionality
- **✍️ Rich Text Editor:** Integrated a WYSIWYG (What You See Is What You Get) editor for an intuitive post-writing experience, allowing for easy formatting like bold, italics, lists, headings, etc.
- **🌍 Multi-Language Support:** The blog is designed to handle both Arabic (RTL) and English (LTR) content seamlessly.
- **Full Post Management (CRUD):** Create, Read, Update, and Delete posts.
- **Image Uploads:** Easily upload and associate a main image with each post.
- **Dynamic Post Details:** View posts with their content, author details, categories, and a full comment section.
- **Smart Pagination:** The main posts list is paginated for better performance and user experience.
- **Category System:**
    - Admins can create and manage categories.
    - Posts can be assigned to one or more categories for easy organization.
- **Commenting System:** Registered users can leave comments on posts.

### 🔐 Authentication & Authorization (Built with ASP.NET Core Identity)
- **Secure User Accounts:** Standard user registration and login functionality.
- **Custom User Profiles:** Users have a `DisplayName` and can have a `ProfileImagePath`.
- **Role-Based Access Control (RBAC):** The application has distinct roles (`Admin`, `Author`, `User`) with different permissions.
- **Fine-Grained Permissions:**
    - **Guest:** Can only read posts.
    - **Authenticated User:** Can create posts and comment.
    - **Post Owner:** Can edit or delete **only their own** posts.
    - **Admin:** Has full control over **all** posts, users, and categories.

### 🛠️ Admin Control Panel
A secure dashboard accessible only to users with the "Admin" role.
- **User Management Dashboard:** View a list of all registered users in the system.
- **Role Management:**
    - See the roles assigned to each user.
    - Easily **promote** or **demote** users by assigning/un-assigning roles (e.g., promote a user to `Author` or `Admin`).

---

## 🚀 Technologies Used

- **Backend:**
    - .NET 8
    - ASP.NET Core MVC
    - Entity Framework Core 8
- **Database:**
    - SQLite (for development flexibility)
- **Authentication:**
    - ASP.NET Core Identity
- **Frontend:**
    - Razor Views
    - Bootstrap (for styling and layout)

---

## 🏁 Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites
- .NET 8 SDK

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
    This will create the `SimpleBlog.db` SQLite database file with the required schema.
    ```sh
    dotnet ef database update
    ```
5.  **Run the application:**
    ```sh
    dotnet run
    ```
The application will be accessible at `https://localhost:port` and `http://localhost:port` where `port` is specified in the console output.

### 🔑 Admin Credentials
The application automatically seeds the database with roles and attempts to assign an admin role.
- **Admin Email:** `youssef@gmail.com`
- **The password:** Contact me through my email `youssef.nour1612@gmail.com` so I give it to you.
