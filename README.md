
# 📅 Event Manager

Event Manager is a full-stack web application designed to simplify event creation, management, and attendance tracking. Built with a **Vue.js + TypeScript frontend** and an **ASP.NET Core Minimal API backend** with **MSSQL Database**, it provides an intuitive user experience.

---

## 🚀 Features

- 🔐 **Authentication & Authorization**
  - **Token-based** login/register system
  - Roles: **Administrator** (Event organizer) and **User**

- 📅 **Event Management**
  - Create, update, delete and **duplicate**
  - View all events or **only joined** events
  - Create **flexible web forms** for each event

- 👥 **User Interaction**
  - **Email notifications** for changes or deletion of an event
  - Simple **UX** design

- 🛠️ **Admin Tools**
  - **Manage** user roles
  - See **all submissions** for an event
  - Export submissions as **.csv** or **.xlsx**

---

## 🧱 Tech Stack

| Frontend  | Backend             | Database | Other            |
|-----------|---------------------|----------|------------------|
| `Vue.js`       | `ASP.NET Core`       | `MSSQL Server` | `Docker` |
| `TypeScript` | `Entity Framework` |   `Docker (containerized)`     | `Swagger UI`     |

---

## 🖥️ Setup Instructions

1. **Clone the repository:**
Run this command in your desired folder or use the built-in Visual Studio Code or Visual Studio *Clone repository* function:

   ```bash
    git clone https://github.com/NValchanov09/Event-Manager.git
   ```
2. **Setup docker composer file:**
Copy the `docker-compose.template.yml` file and rename it `docker-compose.yml`. 
- Change the Database user password "**YourDbPassword**" to your password (In both the *DefaultConnection* string and *SA_PASSWORD*)
- Change the MailSender email "**YourMailSenderEmail**" to your email
- Change the MailSender password "**YourMailSenderPassword**" to your password

3. **Run docker composer** (Backend and MSSQL Server):
Open *Docker Desktop* first and keep it open.
Run this command in the `/Event-Manager/Backend` folder:

 ```bash
  docker-compose down -v
  docker-compose up --build
   ```
4. **Run frontend**
Download **Node.js** if already haven't.
Run this command in the `/Event-Manager/Frontend` folder:
```bash
 npm install
 npm run dev
```
5. **Open the event manager**
Open directly from the link from the terminal (from `npm run dev`) or open it through the browser. In my case: http://localhost:5173/
