🏥 Clinic Management System (CMS)
A full-stack web application built with ASP.NET Core Web API and Angular, designed to streamline clinic operations including patient management, doctor scheduling, appointments, and billing — with secure role-based access for Admin, Doctor, and Receptionist.

🚀 Tech Stack
LayerTechnologyBackendASP.NET Core Web API, C#FrontendAngular, TypeScript, BootstrapDatabaseSQL ServerData AccessADO.NET, LINQ, Stored ProceduresAuthenticationJWT (JSON Web Tokens)ToolsVisual Studio, VS Code, Postman, Git

✨ Features

Role-Based Access Control — Three roles: Admin, Doctor, and Receptionist, each with controlled module-level permissions
Patient Management — Add, view, update, and delete patient records
Doctor Management — Manage doctor profiles and availability
Appointment Scheduling — Book, update, and cancel appointments
Billing Module — Generate and manage patient billing records
JWT Authentication — Secure login with token-based authentication
Responsive UI — Mobile-friendly interface built with Bootstrap and Angular


🏗️ Architecture
ClinicManagementSystem/
├── Backend (ASP.NET Core Web API)
│   ├── Controllers/        # API endpoints
│   ├── Services/           # Business logic layer
│   ├── Repositories/       # Data access layer (ADO.NET)
│   ├── Models/             # Entity models
│   └── Helpers/            # JWT, error handling, validation
│
└── Frontend (Angular SPA)
    ├── components/         # UI components per module
    ├── services/           # HTTP services (API calls)
    ├── guards/             # Route guards for role-based access
    └── interceptors/       # Global HTTP error interceptors

🔐 User Roles
RoleAccessAdminFull access — manage doctors, patients, billing, and usersDoctorView assigned appointments and patient recordsReceptionistBook appointments and manage patient check-ins

⚙️ Getting Started
Prerequisites

.NET 8 SDK
Node.js & npm
SQL Server
Angular CLI — npm install -g @angular/cli

Backend Setup
bash# Clone the repository
git clone https://github.com/Ramalingamsj/ClinicManagementSystem.git
cd ClinicManagementSystem

# Restore dependencies
dotnet restore

# Update connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ClinicDB;Trusted_Connection=True;"
}

# Run the API
dotnet run
Frontend Setup
bashcd ClientApp   # or your Angular folder name

# Install dependencies
npm install

# Start Angular app
ng serve
Open your browser at http://localhost:4200

📡 API Endpoints
MethodEndpointDescriptionPOST/api/auth/loginUser login — returns JWT tokenGET/api/patientsGet all patientsPOST/api/patientsAdd new patientPUT/api/patients/{id}Update patientDELETE/api/patients/{id}Delete patientGET/api/appointmentsGet all appointmentsPOST/api/appointmentsBook appointmentGET/api/billingGet billing records

🗄️ Database

Normalized SQL Server schema with primary keys, foreign keys, and indexing
Queries handled via ADO.NET stored procedures and parameterized queries
Supports 500+ patient records with optimized performance


👨‍💻 Author
Ramalingam S J

GitHub: @Ramalingamsj
LinkedIn: linkedin.com/in/ramalingam-s-j-a494b030b
Email: ramalingamsj21@gmail.com


📄 License
This project is open source and available under the MIT License.
