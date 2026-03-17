# SongShuVegetable

模拟植物大战僵尸 —— Unity 6 Demo 项目

## 环境要求

- Unity 6000.0.x（通过 Unity Hub 安装）
- Node.js 18+（用于运行 MCP 服务）
- Cursor IDE

## Cursor MCP 配置（换电脑必读）

本项目使用了 **Unity MCP** 和 **Filesystem MCP**，让 Cursor AI 可以直接操作 Unity Editor 和项目文件。

由于 MCP 配置包含本机路径，**`.cursor/mcp.json` 不会提交到 git**，需要在每台电脑上手动创建。

在项目根目录创建 `.cursor/mcp.json`，填入以下内容并替换路径：

```json
{
  "mcpServers": {
    "unity": {
      "command": "npx",
      "args": [
        "-y",
        "@nurture-tech/unity-mcp-runner",
        "-unityPath",
        "/Applications/Unity/Hub/Editor/<你的Unity版本>/Unity.app/Contents/MacOS/Unity",
        "-projectPath",
        "<本机项目路径>/SongShuVegetable"
      ]
    },
    "filesystem": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-filesystem",
        "<本机项目路径>/SongShuVegetable"
      ]
    }
  }
}
```

配置完成后**重启 Cursor** 生效。Unity MCP 需要 Unity Editor 已打开该项目。

## 项目目录结构

```
Assets/
├── Art/            # 美术资源（Sprites / Materials / Models / UI）
├── Audio/          # 音频（BGM / SFX）
├── Prefabs/        # 预制体（Plants / Zombies / Projectiles / UI）
├── Scenes/         # 场景文件
├── ScriptableObjects/  # 数据配置（Plants / Zombies）
├── Scripts/        # C# 脚本
│   ├── Core/       # 核心系统（GameManager、ObjectPool）
│   ├── Plants/     # 植物逻辑
│   ├── Zombies/    # 僵尸逻辑
│   ├── UI/         # UI 脚本
│   ├── Data/       # ScriptableObject 定义
│   └── Utils/      # 工具类
└── Settings/       # 项目设置
```
