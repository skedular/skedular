using System.Text.Json;
using Enterprise.Shared.Mcp.Configurations;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Ai;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IMcpServerBuilder AddMcpServer(IConfiguration configuration, ICollection<Type> types)
        {
            var mcpConfig = configuration.GetSection(McpConfig.Key).Get<McpConfig>();
            ArgumentNullException.ThrowIfNull(mcpConfig);

            var mcpServerBuilder = services
                .AddSingleton(mcpConfig)
                .AddMcpServer()
                .WithHttpTransport(option => option.Stateless = false);

            return types.Aggregate(mcpServerBuilder, (current, type) => current.WithToolsFromAssembly(type.Assembly));
        }
    }

    extension(WebApplication app)
    {
        public WebApplication UseMcpServer()
        {
            var mcpConfig = app.Services.GetService<McpConfig>();
            if (mcpConfig is not null)
            {
                app.MapMcp(mcpConfig.Path);
            }

            return app;
        }
    }

    extension(AgentSession session)
    {
        public string ToSerializedString() => JsonSerializer.Serialize(session.Serialize());
    }

    extension(string? serializedSession)
    {
        public async Task<AgentSession> ToAgentSessionAsync(AIAgent agent, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(serializedSession))
            {
                return await agent.GetNewSessionAsync(cancellationToken);
            }

            return await agent.DeserializeSessionAsync(
                JsonSerializer.Deserialize<JsonElement>(serializedSession),
                cancellationToken: cancellationToken);
        }
    }
}
