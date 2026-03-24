# CatFoodManager (猫粮管理器)

CatFoodManager is a modern desktop hybrid application designed to help owners manage their cat's food inventory, track historical price drops across e-commerce platforms, and maintain records of various cat food brands. It features a complete Clean Architecture-based backend in .NET 10, accompanied by a dynamic, responsive frontend built with Vue 3 and Element Plus, hosted inside a WinForms container using CefSharp.

## 🌟 Key Features

* **Inventory Management**: Keep track of cat food stock, types (e.g., dry food, wet food, snacks), weights, purchase prices, and feeding progress.
* **Best Price Tracking**: Monitor historical low prices across various platforms (Taobao, JD.com, Pinduoduo, etc.) to make informed purchasing decisions.
* **Brand & Factory Registry**: Manage a database of cat food brands and their corresponding OEMs/factories.
* **Smart OCR Integration**: Features built-in Google Gemini AI OCR capabilities to automatically extract and sync product data or prices directly from images or screenshots.
* **Image Management**: First-class support for storing and viewing product and receipt images locally.
* **Modern UI/UX**: A beautiful, performant interface leveraging the Vue 3 ecosystem with Element Plus.

## 🏗 Architecture

The project strictly follows **Clean Architecture** principles to ensure separation of concerns, testability, and long-term maintainability. The user interface uses **Chromium Embedded Framework (CefSharp)** to render a modern web application, communicating with the robust .NET backend via seamless JavaScript bridging.

### Project Structure

```text
CatFoodManager/
├── CatFoodManager.Core/          # Cross-cutting concerns, shared interfaces, AI prompts
├── CatFoodManager.Domain/        # Enterprise domain entities (CatFood, BestPrice, Brand)
├── CatFoodManager.Application/   # Use cases, application services, DTOs, interfaces
├── CatFoodManager.Infrastructure/# Data access (SQLite), caching, Gemini AI API integration
├── CatFoodManager.WebAPI/        # (Optional) REST API host
├── CatfoodManagement/            # Desktop Host (WinForms + CefSharp), JS-Bridge APIs
│   ├── Services/Bridge/          # JS-to-C# communication endpoints
│   ├── catfood-web/              # Vue 3 Frontend source code
│   └── wwwroot/                  # Compiled Vue assets, served by CefSharp
└── CatFoodManager.Tests/         # Unit and Integration test projects
```

## 🛠 Technology Stack

### Backend (.NET 10.0)
* **Framework**: .NET 10.0 Windows Forms
* **Architecture**: Clean Architecture & Domain-Driven Design (DDD) principles
* **Database**: SQLite (via `sqlite-net-pcl` and `SQLiteNetExtensions`)
* **Host Container**: CefSharp.WinForms.NETCore
* **AI & Utilities**: Gemini AI API (for OCR), Microsoft.Extensions.DependencyInjection

### Frontend (User Interface)
* **Framework**: Vue 3.4+ (Composition API & `<script setup>`)
* **Language**: TypeScript 5.x
* **UI Library**: Element Plus 2.13+
* **State Management**: Pinia 2.x
* **Routing**: Vue Router 4.x
* **Build Tool**: Vite 5.x
* **Styling**: SCSS / Sass

## 🚀 Getting Started

### Prerequisites
* **.NET 10.0 SDK** or later
* **Node.js** (v18.0 or later) and npm
* **Visual Studio 2022** (v17.10+) or JetBrains Rider

### Setup & Build

1. **Clone the repository**:
   ```bash
   git clone <your-repo-url>
   cd CatFoodManager/CatFoodManager
   ```

2. **Configure Application Settings**:
   Navigate to the host project `CatfoodManagement` and copy the example configuration:
   ```bash
   cd CatfoodManagement
   cp appsettings.example.json appsettings.json
   ```
   Edit `appsettings.json` to insert your valid Gemini API Key (`Gemini:ApiKey` and `AppSettings:AI:ApiKey`) for the smart OCR features to function.

3. **Install Frontend Dependencies**:
   ```bash
   cd catfood-web
   npm install
   ```
   *(Note: The main `.sln` build process is configured to automatically run `npm run build` during compilation, so you typically do not need to manually build the frontend unless developing the UI independently).*

4. **Run the Application**:
   Open `CatFoodManager.sln` in Visual Studio and set **`CatfoodManagement`** as the startup project. Alternatively, use the CLI:
   ```bash
   dotnet restore
   dotnet run --project CatfoodManagement/CatfoodManagement.csproj
   ```
   The local SQLite database will be automatically created on the first launch.

## 🤝 Development Workflow

### Frontend Development
To work exclusively on the UI with Hot-Module Replacement (HMR):
```bash
cd CatfoodManagement/catfood-web
npm run dev
```
*Note: Some features rely on the CefSharp JavaScript Bridge and might require the backend to be running or APIs to be mocked in a standard browser.*

### Modifying the JS-Bridge
1. Create a new API class in `CatfoodManagement/Services/Bridge/` (e.g., `NewFeatureApi.cs`).
2. Register the API in `JavaScriptBridge.cs`.
3. Add the corresponding TypeScript type definition in `catfood-web/src/utils/bridge.ts` to maintain end-to-end type safety.

## 📄 License

This project is licensed under the MIT License.
