# 💞 Connectly API

Connectly is a **modern dating application** built with **.NET Web API**, providing a secure, scalable, and feature-rich backend that powers the Connectly frontend.  
It supports **real-time messaging, user matching, authentication, and photo management** through Cloudinary.

---

## 🚀 Features

- 🧑‍🤝‍🧑 **User Registration & Authentication**
  - JWT & Refresh Token authentication
  - Role-based authorization
  - Token blacklisting using **Redis**

- 🖼️ **Photo Management**
  - Upload, set main photo, delete photos via **Cloudinary**

- 💬 **Real-Time Messaging**
  - Implemented using **SignalR**
  - Message seen tracking
  - Online presence status tracking

- ❤️ **Like System**
  - Users can like other members
  - Filtered lists for:
    - People you like
    - People who liked you
    - Mutual likes

- 🔎 **Member Filtering & Pagination**
  - Filter by gender, age range, and sort order (Last Active / Newest)
  - Paginated member list for optimized performance

- 🧱 **Entity Framework Core**
  - Code-First migrations with SQL Server
  - Repository & Unit of Work pattern
  - Clean Architecture with Core, Infrastructure, and API layers

---
## 🏗️ Project Structure
Connectly.API
│── Controllers
│── DTOs
│── Errors
│── Extensions
│── Helpers
│── Middlewares
│── SingalR
│── appsettings.json
│── Program.cs
│── GlobalUsings.cs

Connectly.Core
│── Dtos
│── Entities
│── Repositories.Contracts
│── Services.Contracts
│── Specifications

Connectly.Infrastructure
│── Data
│── Migrations
│── Repositories
│── SpecificationsEvaluator.cs

Connectly.Service
│── Photos
│── Authservice.cs
│── Tokenblacklistservice.cs

## ⚙️ Technologies Used

- **.NET 8 Web API**
- **Entity Framework Core**
- **SQL Server**
- **Redis** (for token blacklisting & caching)
- **SignalR** (for real-time chat & presence)
- **Cloudinary** (for image storage)
- **JWT Authentication with Refresh Tokens**

---

## 🔐 Authentication Flow

1. User registers and receives access & refresh tokens.
2. Access token expires quickly; refresh token used to obtain a new one.
3. Blacklisted tokens stored in **Redis** for logout & security.
4. Secured endpoints using JWT bearer authentication.

---

## 🗄️ Database Design

- **Users**
  - Personal details, photos, likes, messages
- **Likes**
  - Many-to-many relationship between users
- **Messages**
  - Tracks sender, receiver, timestamps, and seen status
- **Photos**
  - Integrated with Cloudinary (main photo support)

