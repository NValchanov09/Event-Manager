
# 📅 Event Manager

## Overview

Event Manager is a full-stack web application designed by me and 9 other people for [**Speed IT Up 2025**](https://careers.nemetschek.bg/speeditup) by [**Nemetschek Bulgaria**](https://www.nemetschek.bg/).

---

## ⏳ Time taken

**Event Manager** was originally developed from **July 4, 2025** to **July 13, 2025**. 

The project was presented at **July 14, 2025**.

This repository was created as a **copy** of the original repository.

I ([**NValchanov09**](https://github.com/NValchanov09)) worked on this repository from **July 15, 2025** to **nowadays**.

---

## 🗝️ Key Features

- Create **flexible forms** for each event (like in **Google Forms** or **Microsoft Forms**)
  
- Add optional sign-up **deadline** for an event

- **Email notifications** to users signed-up for an event after a change or deletion of the event
  
- Export event data and submissions for an event as a **.csv** or **.xlsx** file

- Simple and responsive **UX** design
  
- **Bulgarian Localization**

- **Role-based authorization** - divided into **Administrator** (Event organizer) and **User**

---

## 🖥️ Tech Stack

| Frontend     | Backend                 | Other           |
|--------------|-------------------------|-----------------|
| `Vue.js`     | `ASP.NET Core` (.NET 9) | `Git/Github`    |
| `TypeScript` | `Entity Framework`      | `Docker`        |
| `Tailwind`   | `MailKit`               | `Swagger UI`    |
| `HTML+CSS`   | `MSSQL`                 | `Node.js (npm)` |

---

## 🖥️ Local Deployment Instructions

> [!IMPORTANT]
>  - For local deployment you need these prerequisites installed on the newest version:
>    
>    - Any coding IDE
>    - Git
>    - Docker Desktop
>    - WSL (Windows Subsystems for Linux)
>    - Node.js



## 1. **Clone the repository**:

Open Terminal and navigate to your desired folder where you want to clone this repository and run the command below:

  ```bash
  git clone https://github.com/NValchanov09/Event-Manager.git
   ```

or

Use the built-in [**Visual Studio Code**](https://code.visualstudio.com/docs/sourcecontrol/overview) or [**Visual Studio**](https://learn.microsoft.com/en-us/visualstudio/version-control/git-with-visual-studio?view=vs-2022) *Clone repository* function.

## 2. **Setup the docker composer file**:

Go to `/Event-Manager/Backend` folder.

Duplicate the `docker-compose.template.yml` file in the same folder and rename it `docker-compose.yml`. 

- Change the Database user password "**YourDbPassword**" to your password (In both the *DefaultConnection* string and *SA_PASSWORD*)
- Change the MailSender email "**YourMailSenderEmail**" to your email
- Change the MailSender password "**YourMailSenderPassword**" to your password

> [!CAUTION]
> If you do not type valid MailSender Email and Password some functionalities might not work or give errors.

> [!IMPORTANT]
> I advise you to create a new email (preferably not in Gmail) because you might be flagged as a spammer or reach the email limit.

## 3. **Run the docker composer**:
   
Open *Docker Desktop* first and keep it open.

Open Terminal and navigate to the `/Event-Manager/Backend` folder.

> Run this commnad if needed to update WSL:
```bash
  wsl --update
   ```

Run this command to conteinerize the Backend and MSSQL Database as a Docker compose.

 ```bash
  docker-compose down -v
  docker-compose up --build
   ```

## 4. **Run frontend**

Open Terminal and navigate to `/Event-Manager/Frontend` folder
   
> Run this command if already haven't:

```bash
  npm install
   ```

Run this command to start the frontend (opens at http://localhost:5173/)

```bash
  npm run dev
   ```

## 5. **Enjoy the application**
   
The start of the container should had also ran the seeder which creates mock data (users, events and submissions).

Now you can either:
- Register with your email and password and then log-in
- Log-in as the God Admin

Here are the admin credentials:

- Email : admin@example.com
- Password : Admin123!


If you logged in the administrator account, from the users menu in the navigation bar you can make other users administrators.

---

> [!NOTE]
> Thanks for reading all this (or atleast some of it) and using this application. ❤️
