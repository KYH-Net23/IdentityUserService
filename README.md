# IdentityService / User Database

IdentityService is a microservice that provides identity management features, including user authentication, role-based access control, and customer data management. It integrates with ASP.NET Core Identity and provides both customer and admin functionalities, secured via roles.

---

## Table of Contents
- [Overview](#overview)
- [Usage](#usage)
  - [Admin Endpoints](#admin-endpoints)
  - [Customer Endpoints](#customer-endpoints)
  - [Admin Login](#admin-login)
  - [Customer Login](#customer-login)
- [User Roles](#user-roles)

---

## Overview

The IdentityService project is designed for managing users with both "Admin" and "Customer" roles. It includes two main controllers:
- **AdminController**: Provides endpoints for creating, retrieving, and deleting users, available only to users with the Admin role.
- **CustomerController**: Manages customer data, with endpoints for retrieving customer information.

### Key Components

- **UserManager** and **SignInManager** from ASP.NET Core Identity for handling user data and authentication.
- **DataContext**: An `IdentityDbContext` that stores `Customer` and `Admin` entities, with additional custom properties.
- **Services**: Includes `CustomerService` and `AdminService` for handling customer and admin data.
- **Factories**: Used to create structured responses for customer data (`CustomerRequestResponseFactory`).

---

## Usage

### Admin Endpoints

1. **GET** `/Admin/{email}`  
   Retrieve a specific user by email.
   - **Authorization**: Admin role required.
   - **Path Parameter**:
     - `email` (string): Email of the user to retrieve.
   - **Response**:
     - 200 OK: Returns the user's email and username.
     - 400 Bad Request: User not found.

2. **GET** `/Admin`  
   Retrieve a list of all users.
   - **Authorization**: Admin role required.
   - **Response**:
     - 200 OK: Returns a placeholder message ("List of users").

3. **POST** `/Admin/create`  
   Create a new customer user.
   - **Authorization**: Admin role required.
   - **Request Body**:
     ```json
     {
       "Username": "sampleuser",
       "Email": "sampleuser@example.com",
       "Password": "Password123!",
       "PhoneNumber": "1234567890",
       "Address": "123 Sample Street"
     }
     ```
   - **Response**:
     - 200 OK: Confirmation message ("User created").
     - 400 Bad Request: If validation fails.

4. **DELETE** `/Admin/delete/{userId}`  
   Delete a user by their ID.
   - **Authorization**: Admin role required.
   - **Path Parameter**:
     - `userId` (string): ID of the user to delete.
   - **Response**:
     - 200 OK: Returns "Ok. Not implemented yet."

### Customer Endpoints

1. **GET** `/api/Customer/GetAll`  
   Retrieve a list of demo customers.
   - **Response**:
     - 200 OK: Returns a list of customer data, including ID, address, email, phone number, and username.

2. **GET** `/api/Customer/GetNull`  
   Test endpoint that returns `null`.
   - **Response**:
     - 200 OK: Returns `null`.

### Admin Login

1. **POST** `/AdminLogin/login`  
   Authenticate an admin user.
   - **Request Body**:
     ```json
     {
       "Email": "admin@example.com",
       "Password": "AdminPass123!"
     }
     ```
   - **Response**:
     - 200 OK: Returns user ID, email, and roles if successful.
     - 400 Bad Request: If authentication fails.

### Customer Login

1. **POST** `/api/CustomerLogin/login`  
   Authenticate a customer user.
   - **Request Body**:
     ```json
     {
       "Email": "customer@example.com",
       "Password": "CustomerPass123!"
     }
     ```
   - **Response**:
     - 200 OK: Returns user ID, email, and roles if successful.
     - 400 Bad Request: If authentication fails.

---

## User Roles

- **Admin**: Has access to all admin functions, including user creation, retrieval, and deletion.
- **Customer**: Accesses only customer-specific data. Customers can only log in and access endpoints designated for customer roles.

Roles are managed by the `UserRoles` enum, which includes:
- `Admin`
- `Customer`
- `Debug`

---
