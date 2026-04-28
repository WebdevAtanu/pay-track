# Payroll Management System

The Payroll Management System is a web-based application built using ASP.NET Core MVC (.NET 8.0). It is designed to streamline the management of employee records, attendance tracking, department organization, and salary processing within an organization.

The system provides separate interfaces for administrators and employees, ensuring secure and role-based access to data.

## Features

### Administration Module

- Manage employee records including creation, updates, and deletion
- Create and organize departments
- Track and manage employee attendance
- Handle salary structures and monthly payroll calculations
- View overall system data through a centralized dashboard

### Employee Module

- Access personal profile information
- View attendance history
- Check salary details and breakdowns

### Authentication and Security

- Secure login system using cookie-based authentication
- Password hashing implemented with BCrypt

### Dashboard

- Overview of total employees
- Summary of salary distribution

## Technology Stack

- Backend Framework: ASP.NET Core MVC (.NET 8.0)
- ORM: Entity Framework Core
- Database: Microsoft SQL Server
- Security: BCrypt password hashing

## Project Structure

The application follows a standard MVC architecture:

- Models: Represents the core data entities
- Views: User interface components
- Controllers: Handles request processing and business logic

The project is organized into two main areas:

- Admin Area: Full access to manage system data
- Employee Area: Limited access for personal data viewing

## Database Design

The system includes the following core entities:

- Employee: Stores employee details such as name, contact information, and department association
- Department: Defines organizational units within the company
- Attendance: Tracks daily attendance records for employees
- Salary: Contains monthly salary information including basic pay, allowances, bonuses, and deductions
