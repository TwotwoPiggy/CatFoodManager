# CatFoodManager / 猫粮管理器

*[English Version Below / 英文版在下方](#english-version)*

---

## 🇨🇳 中文版 (Chinese Version)

CatFoodManager 是一个现代化的桌面混合应用程序，旨在帮助猫主人管理猫粮库存，追踪各大电商平台的历史降价情况，并维护各种猫粮品牌的数据。它包含基于 .NET 10 的完整整洁架构 (Clean Architecture) 后端，以及使用 Vue 3 和 Element Plus 构建的动态、响应式前端（通过 CefSharp 托管在 WinForms 容器中）。

### 🌟 核心功能

* **库存管理**: 记录猫粮库存、类型（如干粮、湿粮、零食）、重量、购买价格以及投喂进度。
* **底价追踪**: 监控各平台（淘宝、京东、拼多多等）的历史最低价格，做出明智的购买决策。
* **品牌与代工厂库**: 管理猫粮品牌及其对应代工厂的数据库。
* **智能 OCR 集成**: 内置 Google Gemini AI OCR 功能，可直接从图片或截图中自动提取和同步产品数据或价格。
* **图片管理**: 原生支持在本地存储和查看产品及票据图片。
* **现代 UI/UX**: 利用 Vue 3 和 Element Plus 打造美观、高性能的用户界面。

### 🏗 架构设计

本项目严格遵循**整洁架构 (Clean Architecture)** 原则，以确保关注点分离、可测试性以及长期的可维护性。用户界面使用 **CefSharp** 渲染现代 Web 应用，并通过无缝的 JavaScript 桥接与强大的 .NET 后端进行通信。

**项目结构:**
```text
CatFoodManager/
├── CatFoodManager.Core/          # 跨层通用、共享接口、AI 提示词
├── CatFoodManager.Domain/        # 领域实体 (CatFood, BestPrice, Brand)
├── CatFoodManager.Application/   # 用例、应用服务、DTOs、接口
├── CatFoodManager.Infrastructure/# 数据访问(SQLite)、缓存、Gemini AI 接口集成
├── CatFoodManager.WebAPI/        # (可选) REST API 宿主
├── CatfoodManagement/            # 桌面宿主 (WinForms + CefSharp), JS-Bridge API
│   ├── Services/Bridge/          # JS 到 C# 的通信端点
│   ├── catfood-web/              # Vue 3 前端源码
│   └── wwwroot/                  # 编译后的 Vue 静态文件，由 CefSharp 提供服务
└── CatFoodManager.Tests/         # 单元测试和集成测试项目
```

### 🛠 技术栈

**后端 (.NET 10.0)**
* 框架: .NET 10.0 Windows Forms
* 架构: 整洁架构 & 领域驱动设计 (DDD)
* 数据库: SQLite (基于 `sqlite-net-pcl` 和 `SQLiteNetExtensions`)
* 宿主容器: CefSharp.WinForms.NETCore
* AI & 工具: Gemini AI API (用于 OCR), 微软依赖注入

**前端 (用户界面)**
* 框架: Vue 3.4+ (组合式 API & `<script setup>`)
* 语言: TypeScript 5.x
* UI 库: Element Plus 2.13+
* 状态管理: Pinia 2.x
* 路由: Vue Router 4.x
* 构建工具: Vite 5.x

### 🚀 快速开始

**开发环境要求**
* **.NET 10.0 SDK** 或更高版本
* **Node.js** (v18.0 或更高版本) 及 npm
* **Visual Studio 2022** (v17.10+) 或 JetBrains Rider

**安装与运行**

1. **克隆仓库**:
   ```bash
   git clone <your-repo-url>
   cd CatFoodManager/CatFoodManager
   ```

2. **配置应用设置**:
   进入宿主项目 `CatfoodManagement`，复制配置示例：
   ```bash
   cd CatfoodManagement
   cp appsettings.example.json appsettings.json
   ```
   编辑 `appsettings.json`，填入有效的 Gemini API 密钥 (`Gemini:ApiKey` 和 `AppSettings:AI:ApiKey`)，以使智能 OCR 功能生效。

3. **运行应用程序**:
   在 Visual Studio 中打开 `CatFoodManager.sln`，将 **`CatfoodManagement`** 设为启动项目即可运行（编译时会自动构建前端 Vue 项目）。或者使用 CLI：
   ```bash
   dotnet restore
   dotnet run --project CatfoodManagement/CatfoodManagement.csproj
   ```

---

<span id="english-version"></span>

## 🇺🇸 English Version

CatFoodManager is a modern desktop hybrid application designed to help owners manage their cat's food inventory, track historical price drops across e-commerce platforms, and maintain records of various cat food brands. It features a complete Clean Architecture-based backend in .NET 10, accompanied by a dynamic, responsive frontend built with Vue 3 and Element Plus, hosted inside a WinForms container using CefSharp.

### 🌟 Key Features

* **Inventory Management**: Keep track of cat food stock, types (e.g., dry food, wet food, snacks), weights, purchase prices, and feeding progress.
* **Best Price Tracking**: Monitor historical low prices across various platforms (Taobao, JD.com, Pinduoduo, etc.) to make informed purchasing decisions.
* **Brand & Factory Registry**: Manage a database of cat food brands and their corresponding OEMs/factories.
* **Smart OCR Integration**: Features built-in Google Gemini AI OCR capabilities to automatically extract and sync product data or prices directly from images or screenshots.
* **Image Management**: First-class support for storing and viewing product and receipt images locally.
* **Modern UI/UX**: A beautiful, performant interface leveraging the Vue 3 ecosystem with Element Plus.

### 🏗 Architecture

The project strictly follows **Clean Architecture** principles to ensure separation of concerns, testability, and long-term maintainability. The user interface uses **Chromium Embedded Framework (CefSharp)** to render a modern web application, communicating with the robust .NET backend via seamless JavaScript bridging.

**Project Structure:**
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

### 🛠 Technology Stack

**Backend (.NET 10.0)**
* Framework: .NET 10.0 Windows Forms
* Architecture: Clean Architecture & Domain-Driven Design (DDD) principles
* Database: SQLite (via `sqlite-net-pcl` and `SQLiteNetExtensions`)
* Host Container: CefSharp.WinForms.NETCore
* AI & Utilities: Gemini AI API (for OCR), Microsoft.Extensions.DependencyInjection

**Frontend (User Interface)**
* Framework: Vue 3.4+ (Composition API & `<script setup>`)
* Language: TypeScript 5.x
* UI Library: Element Plus 2.13+
* State Management: Pinia 2.x
* Routing: Vue Router 4.x
* Build Tool: Vite 5.x

### 🚀 Getting Started

**Prerequisites**
* **.NET 10.0 SDK** or later
* **Node.js** (v18.0 or later) and npm
* **Visual Studio 2022** (v17.10+) or JetBrains Rider

**Setup & Build**

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

3. **Run the Application**:
   Open `CatFoodManager.sln` in Visual Studio and set **`CatfoodManagement`** as the startup project. The frontend will be built automatically during compilation. Alternatively, use the CLI:
   ```bash
   dotnet restore
   dotnet run --project CatfoodManagement/CatfoodManagement.csproj
   ```

---
## 📄 License
This project is licensed under the MIT License.
