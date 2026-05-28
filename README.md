# 📊 Hybrid AI Pipeline - Real-Time Telemetry Inference System

![.NET](https://img.shields.io/badge/.NET-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?logo=react&logoColor=61DAFB)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)
![gRPC](https://img.shields.io/badge/gRPC-244C5A?logo=grpc&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-0078D4?logo=microsoft&logoColor=white)

## 📖 About

The project is a high-performance, microservices-based architecture designed for live data analysis and visualization.

Built with a **.NET 9** Gateway, **Rust** Inference Engine, and **React**, this project demonstrates a modern **Polyglot Microservices** approach. It seamlessly integrates a secure web dashboard with an isolated mathematical computation engine, calculating metrics like **Shannon Entropy** and **Eccentricity** from raw binary data and streaming results back in real-time.

## 🛠 Tech Stack

- **Inference Engine (Core):** Rust, gRPC (Tonic, Prost)
- **API Gateway:** C#, ASP.NET Core 9 Minimal API
- **Frontend Dashboard:** React 18, TypeScript, Vite, Tailwind CSS, Recharts
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (Code-First)
- **Security:** JWT Authentication, BCrypt
- **Real-Time Communication:** SignalR (WebSockets)
- **Inter-Service Communication:** gRPC (Protocol Buffers)

## ✨ Highlights & Features

### 🔐 Advanced Security

- **JWT Authentication:** Secure, token-based authorization for the .NET 9 Gateway.
- **Engine Isolation:** The Rust engine has zero exposure to the internet and zero access to the database. It strictly accepts structured gRPC requests from the authenticated gateway.
- **Audit Logging:** Every inference request is tracked and persisted in PostgreSQL, logging timestamp, user ID, computed metrics, and anomaly flags.

### 🏗 Architecture & Performance

- **Polyglot Microservices:** Best-tool-for-the-job approach, utilizing Rust for deterministic, high-performance math processing and C# for robust API orchestration.
- **Blazing Fast Inter-Service Communication:** The .NET Gateway and Rust Engine communicate via HTTP/2 using Protocol Buffers (gRPC), ensuring minimal latency and strict type safety.
- **Minimal APIs:** Lightweight and fast endpoints in .NET 9 for file uploads and authentication.

### 📦 Domain Logic

- **Live Telemetry Engine:** Computes complex mathematical features (Shannon Entropy, Eccentricity) directly from raw byte streams.
- **Raw Binary Handling:** Users can securely upload raw `.bin` or `.txt` baseline telemetry data (`multipart/form-data`) to be stored in the database and serialized to the engine.
- **Real-Time WebSockets Streaming:** SignalR Hubs push live telemetry data directly from the backend to the React frontend, allowing `Recharts` to draw dynamic, shifting graphs without client-side polling.

## 🚀 Quick Start

To run this system locally:

1.  **Clone the repo:**
    ```bash
    git clone https://github.com/OsherBerGit/High-Performance-AI-Inference-System.git
    cd High-Performance-AI-Inference-System
    ```
2.  **Database Setup:**
    Update the `DefaultConnection` string in `gateway/appsettings.json` with your PostgreSQL credentials.
3.  **Apply Migrations:**
    ```bash
    cd gateway
    dotnet ef database update
    ```
4.  **Start the Rust Inference Engine:**
    The gRPC engine listens on port 50051.
    ```bash
    cd ../engine
    cargo run
    ```
5.  **Start the .NET Gateway:**
    The API and SignalR Hub listen on port 5110.
    ```bash
    cd ../gateway
    dotnet run
    ```
6.  **Start the React Frontend:**
    The Vite server runs on port 5173.
    ```bash
    cd ../dashboard
    npm install
    npm run dev
    ```

---

_Note: Developed as a showcase of polyglot microservices, real-time data streaming, and high-performance computing integration._