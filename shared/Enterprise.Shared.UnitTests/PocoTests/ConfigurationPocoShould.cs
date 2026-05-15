using Enterprise.Shared.Accounting;
using Enterprise.Shared.Ai.Configurations;
using Enterprise.Shared.Azure.Configurations;
using Enterprise.Shared.Email;
using Enterprise.Shared.FileStorage;
using Enterprise.Shared.GraphQL.Configurations;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Payment.Configurations;
using Enterprise.Shared.Security.Sso.Models;
using Enterprise.Shared.Temporal.Configurations;

namespace Enterprise.Shared.UnitTests.PocoTests;

/// <summary>
///     Covers line coverage for simple POCO/configuration classes that are just properties.
/// </summary>
[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ConfigurationPocoShould
{
    [Fact]
    public void TemporalConfiguration_has_expected_defaults()
    {
        var config = new TemporalConfiguration();
        config.Worker.TaskQueue.ShouldBe(string.Empty);
        config.Worker.Capacity.MaxConcurrentWorkflowTaskPollers.ShouldBe(Defaults.CapacityMaxConcurrentWorkflowTaskPollers);
        config.Worker.Capacity.MaxConcurrentWorkflowTaskExecutors.ShouldBe(Defaults.CapacityMaxConcurrentWorkflowTaskExecutors);
        config.Worker.Capacity.MaxConcurrentActivityTaskPollers.ShouldBe(Defaults.CapacityMaxConcurrentActivityTaskPollers);
        config.Worker.Capacity.MaxConcurrentLocalActivityExecutors.ShouldBe(Defaults.CapacityMaxConcurrentLocalActivityExecutors);
        config.Worker.Capacity.MaxConcurrentActivityExecutors.ShouldBe(Defaults.CapacityMaxConcurrentActivityTaskExecutors);
        config.Worker.RateLimits.MaxWorkerActivitiesPerSecond.ShouldBeNull();
        config.Worker.RateLimits.MaxTaskQueueActivitiesPerSecond.ShouldBeNull();
        config.Worker.Cache.MaxInstances.ShouldBe(Defaults.CacheMaxInstances);
        config.Connection.Namespace.ShouldBe(string.Empty);
        config.Connection.Target.ShouldBe(string.Empty);
        config.Connection.Mtls.ShouldBeNull();
    }

    [Fact]
    public void MtlsConfig_roundtrip()
    {
        var mtls = new MtlsConfig("key.pem", "cert.pem");
        mtls.KeyFile.ShouldBe("key.pem");
        mtls.CertChainFile.ShouldBe("cert.pem");
    }

    [Fact]
    public void KafkaTelemetryConfiguration_default()
    {
        var config = new KafkaTelemetryConfiguration();
        config.Enabled.ShouldBeFalse();
    }

    [Fact]
    public void SchemaRegistryConfiguration_defaults()
    {
        var config = new SchemaRegistryConfiguration();
        config.Url.ShouldBe(string.Empty);
        config.ApiKey.ShouldBe(string.Empty);
        config.SecretKey.ShouldBe(string.Empty);
        config.AutoRegisterSchema.ShouldBeTrue();
        config.UseLatestVersion.ShouldBeFalse();
    }

    [Fact]
    public void RetryTopicSetting_defaults()
    {
        var setting = new RetryTopicSetting();
        setting.Topic.ShouldBe(string.Empty);
        setting.RetryDelaySeconds.ShouldBe(0);
    }

    [Fact]
    public void PaginatedInfo_has_all_properties()
    {
        var info = new PaginatedInfo(true, false, "start", "end");
        info.HasNextPage.ShouldBeTrue();
        info.HasPreviousPage.ShouldBeFalse();
        info.StartCursor.ShouldBe("start");
        info.EndCursor.ShouldBe("end");
    }

    [Fact]
    public void EmailAttachment_is_a_record()
    {
        using var stream = new MemoryStream();
        var attachment = new EmailAttachment(stream, "file.pdf", "application/pdf");
        attachment.Name.ShouldBe("file.pdf");
        attachment.MimeType.ShouldBe("application/pdf");
    }

    [Fact]
    public void AzureEntraConfiguration_defaults()
    {
        var config = new AzureEntraConfiguration();
        config.ClientId.ShouldBe(string.Empty);
        config.ClientSecret.ShouldBe(string.Empty);
    }

    [Fact]
    public void McpConfig_defaults()
    {
        var config = new McpConfig();
        config.Path.ShouldBe(string.Empty);
    }

    [Fact]
    public void XeroTokenRefreshResult_roundtrip()
    {
        var result = new XeroTokenRefreshResult(true, false, "access", "refresh", TimeProvider.System.GetUtcNow(),
            TimeProvider.System.GetUtcNow().AddDays(30), null);
        result.IsSuccessful.ShouldBeTrue();
        result.NeedsReconnect.ShouldBeFalse();
        result.AccessTokenEncrypted.ShouldBe("access");
        result.RefreshTokenEncrypted.ShouldBe("refresh");
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void StripeConfiguration_defaults()
    {
        var config = new StripeConfiguration();
        config.SecretKey.ShouldBe(string.Empty);
        config.PublishableKey.ShouldBe(string.Empty);
        config.OAuthClientId.ShouldBe(string.Empty);
        config.OrganizationPlatformAccountWebhookKey.ShouldBe(string.Empty);
    }

    [Fact]
    public void CloudflareConfiguration_defaults()
    {
        var config = new CloudflareConfiguration { CdnBaseUrl = new Uri("https://cdn.example.com") };
        config.AccountId.ShouldBe(string.Empty);
        config.AccessKey.ShouldBe(string.Empty);
        config.SecretKey.ShouldBe(string.Empty);
        config.CdnR2BucketName.ShouldBe(string.Empty);
        config.FileR2BucketName.ShouldBe(string.Empty);
        config.CdnBaseUrl.ShouldBe(new Uri("https://cdn.example.com"));
    }

    [Fact]
    public void IdentityProvidersConfiguration_defaults()
    {
        var config = new IdentityProvidersConfiguration();
        config.Cognito.ShouldBeNull();
        config.Google.ShouldBeNull();
        config.WorkOS.ShouldBeNull();
    }

    [Fact]
    public void IdentityProvidersConfiguration_cognito_and_google_and_workos()
    {
        var config = new IdentityProvidersConfiguration
        {
            Cognito = new Cognito { JwksUri = new Uri("https://cognito.example.com"), Issuer = "iss", Audiences = "aud" },
            Google = new IdentityProviders.Configurations.Google { ApplicationId = "appId", Issuer = "google" },
            WorkOS = new IdentityProviders.Configurations.WorkOS
            {
                JwksUri = new Uri("https://workos.example.com"), Issuer = "workos", ApiKey = "key"
            }
        };

        config.Cognito.JwksUri.ShouldBe(new Uri("https://cognito.example.com"));
        config.Google.ApplicationId.ShouldBe("appId");
        config.WorkOS.ApiKey.ShouldBe("key");
        config.WorkOS.OtherIssuers.ShouldBeEmpty();
    }

    [Fact]
    public void SamlResponse_defaults()
    {
        var response = new SamlResponse();
        response.Destination.ShouldBe(string.Empty);
        response.InResponseTo.ShouldBe(string.Empty);
        response.NameId.ShouldBeNull();
        response.Roles.ShouldBeEmpty();
        response.Issuer.ShouldBe(string.Empty);
        response.StatusCode.ShouldBe(string.Empty);
    }

    [Fact]
    public void GraphqlConfig_defaults()
    {
        var config = new GraphqlConfig();
        config.IncludeCookies.ShouldBeFalse();
        config.NitroEnabled.ShouldBeFalse();
        config.IntrospectionEnabled.ShouldBeFalse();
        config.Path.ShouldBe(string.Empty);
    }

    [Fact]
    public void GraphQL_types_roundtrip()
    {
        var pageInfo = new PageInfo { HasNextPage = true, HasPreviousPage = false, StartCursor = "a", EndCursor = "b" };
        pageInfo.HasNextPage.ShouldBeTrue();
        pageInfo.EndCursor.ShouldBe("b");

        var connection = Connection<PageInfo>.Empty;
        connection.TotalCount.ShouldBe(0);
        connection.Edges.ShouldBeEmpty();
        connection.PageInfo.HasNextPage.ShouldBeFalse();

        var node = new Node("n1");
        node.Id.ShouldBe("n1");

        var nodeEmpty = new Node();
        nodeEmpty.Id.ShouldBe(string.Empty);

        var version = new GraphQL.Types.Version { Major = 1, Minor = 2, Build = 3, Revision = 4 };
        version.Major.ShouldBe(1);
    }

    [Fact]
    public void KafkaOutbox_defaults()
    {
        var outbox = new KafkaOutbox { Id = "1", Topic = "t", Timestamp = TimeProvider.System.GetUtcNow() };
        outbox.RetryCount.ShouldBe(0);
        outbox.Headers.ShouldBeEmpty();
        outbox.Key.ShouldBeEmpty();
        outbox.Payload.ShouldBeEmpty();
        outbox.ProcessingErrors.ShouldBeNull();
    }

    [Fact]
    public void TemporalOutbox_defaults()
    {
        var outbox = new TemporalOutbox { Id = "1", WorkflowType = "wf", Timestamp = TimeProvider.System.GetUtcNow() };
        outbox.RetryCount.ShouldBe(0);
        outbox.ProcessingErrors.ShouldBeNull();
    }

    [Fact]
    public void FileStorageConfiguration_defaults()
    {
        var config = new FileStorageConfiguration();
        config.UseFileServer.ShouldBeTrue();
        config.FileServerPublicFilePath.ShouldBe(string.Empty);
        config.FileServerFilePath.ShouldBe(string.Empty);
        config.FileEndpoint.ShouldBe(string.Empty);
        config.MaxFileSize.ShouldBe(0);
    }
}
