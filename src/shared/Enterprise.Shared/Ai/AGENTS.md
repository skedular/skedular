# Ai Module — Agent Notes

## Purpose

Provides two independent capabilities:

1. **MCP server** (`Mcp/`) — registers a Model Context Protocol server that exposes tools from one or more assemblies
   over HTTP.
2. **Agent session helpers** — extension methods for serializing and deserializing Microsoft Agent Framework
   `AgentSession` objects to/from JSON strings, enabling session persistence across requests.

## Registration — MCP Server

```csharp
services.AddMcpServer(configuration, types: [typeof(MyTool1), typeof(MyTool2)]);

// Map the HTTP transport endpoint
app.UseMcpServer();
```

`AddMcpServer` scans each type's assembly for MCP tool classes and registers them via
`WithToolsFromAssembly`. The HTTP path is read from `Mcp:Path` in configuration.

**Config section key:** `Mcp` — see `Mcp/Configurations/McpConfig.cs`.

```json
{
  "Mcp": {
    "Path": "/mcp"
  }
}
```

`UseMcpServer()` is a no-op when `McpConfig` is not registered, so it is safe to call from hosts that conditionally
enable MCP.

## Agent Session Helpers

```csharp
// Serialize the current session state to a JSON string (e.g. for storage)
string json = await agent.ToSerializedStringAsync(session, cancellationToken);

// Deserialize back, or create a new session if the string is null/empty
AgentSession session = await json.ToAgentSessionAsync(agent, cancellationToken);
```

These helpers live as extension methods on `AIAgent` and `string?` respectively.

## NuGet Dependencies

| Package                           | Purpose                               |
|-----------------------------------|---------------------------------------|
| `ModelContextProtocol.AspNetCore` | MCP HTTP transport and server builder |
| `Microsoft.Agents.AI.AzureAI`     | Azure AI agent framework              |
| `Microsoft.Agents.AI.OpenAI`      | OpenAI agent framework                |
| `Azure.AI.OpenAI`                 | Azure OpenAI SDK                      |
| `OpenAI`                          | OpenAI SDK                            |

## Rules

- MCP tool classes must be in an assembly reachable through one of the `types` passed to
  `AddMcpServer` — scanning is per-assembly, not global.
- The filename `Extentions.cs` is a known typo in the source; do not rename it as other files in this repo may reference
  it. Fix the typo only if all references are updated together.
- Do not add AI model configuration constants here; keep model names and endpoint URLs in the calling host's
  configuration.
