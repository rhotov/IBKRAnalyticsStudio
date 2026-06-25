# IBKR Analytics Studio Web

IBKR Analytics Studio Web 是一个基于浏览器的本地 IBKR Activity Statement 解析器。它可以直接读取本地 CSV/TXT 报表，解析账户 NAV、持仓、交易、盈亏、每日统计等内容，并在浏览器中可视化展示。

## 重点说明

- 仅支持本地文件导入和文本粘贴。
- 不会将报表上传到服务器。
- 不支持浏览器自动拉取 IBKR Flex API 报表。
- 导出 JSON/PNG、图表和分析均在本地完成。

## 使用方式

1. 选择或拖放本地 IBKR Activity Statement CSV/TXT 文件。
2. 或者在“粘贴 CSV 文本”中直接粘贴报表内容。
3. 解析成功后即可查看总览、持仓、收益、每日统计和数据质量。
4. 需要时点击“导出 JSON”或“生成分享图”。

## 推荐配置

- 优先导出英文 IBKR Activity Statement CSV。
- 如果使用 IBKR Flex Query 导出，请确保选择 CSV 并包含必要的字段。
- 本网页版本不自动拉取 Flex API 报表，需先在 IBKR Portal 导出 CSV，再导入。

## 本地开发

```bash
npm run serve
```

打开：

```text
http://127.0.0.1:4187/
```

语法检查：

```bash
npm run check
```

## 在线部署

这是一个静态网站，部署方式很简单：

- 部署根目录下的 `index.html` 和静态资源目录。
- 也可以使用 GitHub Pages、Vercel、Netlify、Cloudflare Pages 等静态宿主。
- 若使用子路径部署，请确保静态资源路径仍可正确访问。

## 目录说明

- `index.html`：单页应用入口。
- `src/`：前端逻辑，包括解析器、编码检测和 UI。
- `assets/`：样式、图标和静态资源。
- `samples/`：示例 CSV 文件。
- `serve.mjs`：本地静态文件服务器。

## 其它

如果需要自动 IBKR Flex API 拉取，请使用 Windows WebView2 桌面版本。

## 目录结构

- `index.html`：单页应用入口。
- `assets/`：样式、图标和静态资源。
- `src/`：前端逻辑，包括解析器、编码检测和 UI。
- `samples/`：示例 CSV 文件。
- `serve.mjs`：本地静态文件服务器。
- `README.md`：项目说明。

## 隐私与安全

- CSV 报表解析在本机完成。
- 报表不会上传到服务器。
- JSON/PNG 导出仅在用户点击时生成。

## 已知限制

- 当前仅支持本地 CSV/TXT 导入或文本粘贴。
- 不支持浏览器内自动拉取 IBKR Flex API 报表。
- 推荐使用英文 IBKR Activity Statement CSV。
- 不同账户权限和 Flex Query 字段配置会影响可解析结果。

## 免责声明

本项目不是 Interactive Brokers 官方产品，也不与 Interactive Brokers LLC 存在官方关联。

请以 IBKR 官方报表为准，自行核对所有投资、税务和会计相关数据。
