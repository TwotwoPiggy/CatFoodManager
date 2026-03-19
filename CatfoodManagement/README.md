# CatFood Manager - CefSharp + Vue 3 重构版本

## 项目结构

```
CatfoodManagement/
├── CatfoodManagement/              # .NET 主项目
│   ├── Services/                   # 服务层
│   │   ├── Bridge/                 # JavaScript 桥接 API
│   │   │   ├── CatFoodApi.cs
│   │   │   ├── BrandApi.cs
│   │   │   └── BestPriceApi.cs
│   │   └── JavaScriptBridge.cs
│   ├── wwwroot/                    # Vue 构建产物
│   ├── MainForm.cs                 # 主窗体
│   ├── Program.cs                  # 程序入口
│   └── appsettings.json            # 配置文件
├── CatfoodManagement.Tests/        # 单元测试项目
└── catfood-web/                    # Vue 3 前端项目
    ├── src/
    │   ├── api/                    # API 接口
    │   ├── assets/                 # 静态资源
    │   ├── components/             # 组件
    │   ├── layouts/                # 布局
    │   ├── router/                 # 路由
    │   ├── stores/                 # 状态管理
    │   ├── types/                  # TypeScript 类型
    │   ├── utils/                  # 工具函数
    │   └── views/                  # 页面视图
    ├── package.json
    ├── vite.config.ts
    └── tsconfig.json
```

## 开发环境要求

### 后端
- .NET 10.0 SDK
- Visual Studio 2022 或 Rider

### 前端
- Node.js 18+
- npm 或 yarn

## 快速开始

### 1. 安装前端依赖

```bash
cd catfood-web
npm install
```

### 2. 开发模式运行前端

```bash
npm run dev
```

### 3. 构建前端项目

```bash
npm run build
```

构建产物将输出到 `../CatfoodManagement/wwwroot` 目录。

### 4. 运行 .NET 项目

```bash
cd ../CatfoodManagement
dotnet restore
dotnet run
```

## 功能模块

### 猫粮库存管理
- 查看猫粮库存列表
- 搜索猫粮（按名称、品牌、ID）
- 分页显示
- 查看产品图片
- 数据同步（OCR 功能）

### 最佳价格管理
- 查看最佳价格列表
- 新增最佳价格记录
- 编辑价格信息
- 购买状态管理
- 图片管理

### 品牌管理
- 品牌列表显示
- 新增品牌
- 编辑品牌
- 删除品牌
- 搜索品牌

## 技术栈

### 后端
- .NET 10.0
- CefSharp.WinForms.NETCore 145.0.260
- Microsoft.Extensions.DependencyInjection
- Newtonsoft.Json

### 前端
- Vue 3.4+
- TypeScript 5.x
- Element Plus 2.6+
- Vue Router 4.x
- Pinia 2.x
- Vite 5.x
- Sass

## CefSharp 通信机制

### C# → JavaScript
```csharp
await browser.EvaluateScriptAsync("window.updateData", data);
```

### JavaScript → C#
```javascript
// 调用 C# API
const result = await window.catFoodApi.GetCatFoods(page, pageSize, searchKey);
const data = JSON.parse(result);
```

## 配置说明

### appsettings.json
```json
{
  "DatabaseSettings": {
    "ConnectionString": "./data/catfood.db"
  },
  "AppSettings": {
    "TessdataPath": "tessdata",
    "PictureFolders": ""
  },
  "CefSettings": {
    "CachePath": "Cache",
    "LogFile": "logs/cef.log",
    "LogSeverity": "Warning",
    "RemoteDebuggingPort": 8088
  }
}
```

## 测试

### 运行单元测试
```bash
cd CatfoodManagement.Tests
dotnet test
```

## 构建发布

### 发布应用程序
```bash
cd CatfoodManagement
dotnet publish -c Release -r win-x64 --self-contained true
```

## 注意事项

1. 首次运行前需要先构建前端项目
2. CefSharp 需要特定的运行时环境，确保安装了 VC++ 运行时
3. 数据库文件会在首次运行时自动创建
4. 图片路径需要在配置文件中正确设置

## 开发指南

### 添加新的 API 接口

1. 在 `CatfoodManagement/Services/Bridge/` 创建新的 API 类
2. 在 `JavaScriptBridge.cs` 中注册 API
3. 在 `catfood-web/src/utils/bridge.ts` 中添加对应的 TypeScript 函数

### 添加新的页面

1. 在 `catfood-web/src/views/` 创建新的 Vue 组件
2. 在 `catfood-web/src/router/index.ts` 中添加路由配置
3. 在布局组件中添加菜单项

## License

MIT
