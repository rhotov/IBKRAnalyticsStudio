# IBKR Analytics Studio WebView2

这是离线版的 Windows 原生壳。它使用 Microsoft Edge WebView2 承载静态前端，并提供 IBKR Flex API 原生桥接。

前端文件会被映射到虚拟 HTTPS 域名：

```text
https://ibkr-analytics.local/index.html
```

这样可以避免 `file://` 下 ES Modules、`fetch()` 和本地资源加载的限制。

## 功能

- 承载离线前端页面。
- 提供 IBKR Flex Web Service 请求桥接。
- 使用 WebView2 本地存储记住 Query ID 和 Token。
- 深色 Windows 标题栏和边框，与应用页面颜色统一。
- 框架依赖或自包含两种发布方式。

## 运行要求

运行框架依赖版本需要：

- Microsoft Edge WebView2 Runtime
- .NET Desktop Runtime 8

构建需要：

- .NET SDK 8

自包含版本不要求目标机器预装 .NET Desktop Runtime，但体积更大。

## 构建

从本目录执行：

```powershell
dotnet restore
dotnet build -c Release
```

发布框架依赖版本：

```powershell
.\build.ps1
```

发布自包含版本：

```powershell
.\build.ps1 -SelfContained
```

发布产物目录：

```text
publish\win-x64-framework-dependent\
```

或：

```text
publish\win-x64-self-contained\
```

## Flex API 桥接

前端通过 WebView2 `postMessage` 发送：

```text
flex.fetch
```

C# 壳调用 IBKR Flex Web Service：

```text
https://ndcdyn.interactivebrokers.com/AccountManagement/FlexWebService/SendRequest
https://ndcdyn.interactivebrokers.com/AccountManagement/FlexWebService/GetStatement
```

使用 Flex Web Service version `3`，并对报表生成中的状态进行有限重试。

返回的 CSV 文本会通过 WebView2 消息发回前端，由 `src/parser.js` 解析。

## Token 处理

Token 和 Query ID 由前端保存在 WebView2 localStorage：

- 不写入 C# 配置文件。
- 不写入源码。
- 不打包进 exe。
- 不输出到日志。

用户可以在页面中点击 **Forget saved** 清除。

## 重要解析约定

当前前端解析器针对 Flex CSV 做了以下处理：

- `CNAV`: 保留全部每日数据，用于每日 TWR 收益率曲线。
- `EQUT`: 取最新 `ReportDate` 作为当前 NAV/现金。
- `POST`: 取最新 `ReportDate`，并优先使用 `LevelOfDetail = SUMMARY`。
- `TRNT`: 只展示 `LevelOfDetail = ORDER`，避免 execution/closed lot 重复。
- `FIFO`: 已实现盈亏按全部日期累计，未实现盈亏取最新日期快照。
- `MTMP`: 取最新 `ReportDate`。

如果 Flex Query 开启 `Breakout by Day`，这套规则可以避免状态型区块被每日快照重复累加。

## 常见问题

### 发布失败，提示文件被占用

如果正在运行旧版 `IBKRAnalyticsStudio.exe`，发布目录里的 dll/exe 会被锁住。关闭应用后重新执行：

```powershell
.\build.ps1
```

### WindowsBase warning

构建时可能出现 `MSB3277 WindowsBase` 版本冲突 warning。这来自 WebView2 包的 WPF 引用，不影响当前 WinForms 壳运行。

### 页面没有使用最新前端

发布目录会复制一份 `wwwroot`。修改 `src/`、`assets/` 或 `index.html` 后，需要重新执行 `build.ps1`。
