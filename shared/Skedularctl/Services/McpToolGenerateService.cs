using System.Text.RegularExpressions;
using System.Xml.Linq;
using CommandLine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Skedularctl.Services;

[Verb("mcp-tool-generate", HelpText = "Generate metadata required by MCP tool")]
// ReSharper disable once ClassNeverInstantiated.Global
public class McpToolGenerateServiceOptions
{
    [Option('i', "input-file", Required = true, HelpText = "Path to the input C# source file that needs to be enriched.")]
    public string InputFilePath { get; init; } = string.Empty;

    [Option('o', "output-file", Required = true, HelpText = "Path to the generated C# file containing MCP annotations.")]
    public string OutputFilePath { get; init; } = string.Empty;
}

public interface IMcpToolGenerateService
{
    Task HandleAsync(McpToolGenerateServiceOptions options, CancellationToken cancellationToken);
}

public class McpToolGenerateService : IMcpToolGenerateService
{
    public async Task HandleAsync(McpToolGenerateServiceOptions options, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.InputFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.OutputFilePath);

        var inputFilePath = Path.GetFullPath(options.InputFilePath);
        if (!Path.Exists(inputFilePath))
        {
            throw new FileNotFoundException($"Unable to locate input file '{inputFilePath}'.", inputFilePath);
        }

        var outputFilePath = Path.GetFullPath(options.OutputFilePath);
        var sourceText = await File.ReadAllTextAsync(inputFilePath, cancellationToken);
        var syntaxTree = CSharpSyntaxTree.ParseText(
            sourceText,
            new CSharpParseOptions(documentationMode: DocumentationMode.Parse),
            inputFilePath,
            cancellationToken: cancellationToken);

        var root = (CompilationUnitSyntax)await syntaxTree.GetRootAsync(cancellationToken);
        var rewriter = new McpToolAttributeRewriter();
        var updatedRoot = (CompilationUnitSyntax)rewriter.Visit(root);

        using var workspace = new AdhocWorkspace();
        var formattedRoot = Formatter.Format(updatedRoot, workspace, cancellationToken: cancellationToken);

        var outputDirectory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        await File.WriteAllTextAsync(outputFilePath, formattedRoot.ToFullString(), cancellationToken);
    }

    private sealed class McpToolAttributeRewriter : CSharpSyntaxRewriter
    {
        private const string DescriptionAttributeName = "System.ComponentModel.Description";
        private const string McpToolAttributeName = "ModelContextProtocol.Server.McpServerTool";
        private const string McpToolTypeAttributeName = "ModelContextProtocol.Server.McpServerToolType";

        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var visitedNode = base.VisitClassDeclaration(node);
            if (visitedNode is not ClassDeclarationSyntax updatedNode)
            {
                return visitedNode;
            }

            if (updatedNode.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                updatedNode = AddMarkerAttribute(updatedNode, McpToolTypeAttributeName);
            }

            return updatedNode;
        }

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var visitedNode = base.VisitMethodDeclaration(node);
            if (visitedNode is not MethodDeclarationSyntax updatedNode)
            {
                return visitedNode;
            }

            if (!updatedNode.Modifiers.Any(SyntaxKind.AbstractKeyword))
            {
                return updatedNode;
            }

            updatedNode = AddMarkerAttribute(updatedNode, McpToolAttributeName);

            var description = ExtractDescription(updatedNode);
            if (!string.IsNullOrWhiteSpace(description))
            {
                updatedNode = AddDescriptionAttribute(updatedNode, description);
            }

            return updatedNode;
        }

        public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var visitedNode = base.VisitPropertyDeclaration(node);
            if (visitedNode is not PropertyDeclarationSyntax updatedNode)
            {
                return visitedNode;
            }

            var description = ExtractDescription(updatedNode);

            if (!string.IsNullOrWhiteSpace(description))
            {
                updatedNode = AddDescriptionAttribute(updatedNode, description);
            }

            return updatedNode;
        }

        private static T AddMarkerAttribute<T>(T node, string attributeName)
            where T : MemberDeclarationSyntax
        {
            if (HasAttribute(node.AttributeLists, attributeName))
            {
                return node;
            }

            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attributeName));
            return (T)node.WithAttributeLists(node.AttributeLists.Add(CreateAttributeList(attribute)));
        }

        private static T AddDescriptionAttribute<T>(T node, string description)
            where T : MemberDeclarationSyntax
        {
            if (HasAttribute(node.AttributeLists, DescriptionAttributeName))
            {
                return node;
            }

            description = description.Replace("///", string.Empty).Trim();

            var attribute = SyntaxFactory
                .Attribute(SyntaxFactory.ParseName(DescriptionAttributeName))
                .WithArgumentList(
                    SyntaxFactory.AttributeArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.AttributeArgument(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    SyntaxFactory.Literal(description))))));

            return (T)node.WithAttributeLists(node.AttributeLists.Add(CreateAttributeList(attribute)));
        }

        private static AttributeListSyntax CreateAttributeList(AttributeSyntax attribute) =>
            SyntaxFactory
                .AttributeList(SyntaxFactory.SingletonSeparatedList(attribute))
                .WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed);

        private static bool HasAttribute(SyntaxList<AttributeListSyntax> attributeLists, string attributeName)
        {
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var normalizedExisting = NormalizeAttributeName(attribute.Name.ToString());
                    var normalizedExpected = NormalizeAttributeName(attributeName);

                    if (string.Equals(normalizedExisting, normalizedExpected, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string NormalizeAttributeName(string attributeName)
        {
            var cleaned = attributeName.StartsWith("global::", StringComparison.Ordinal)
                ? attributeName["global::".Length..]
                : attributeName;

            var lastSegment = cleaned.Split('.').LastOrDefault() ?? cleaned;
            return Regex.Replace(lastSegment, "Attribute$", string.Empty, RegexOptions.CultureInvariant);
        }

        private static string? ExtractDescription(SyntaxNode node)
        {
            var documentationComment = node
                .GetLeadingTrivia()
                .Select(trivia => trivia.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (documentationComment == null)
            {
                return null;
            }

            var rawXml = string.Concat(documentationComment.Content.Select(content => content.ToFullString()));
            if (string.IsNullOrWhiteSpace(rawXml))
            {
                return null;
            }

            try
            {
                var wrapper = XElement.Parse($"<doc>{rawXml}</doc>");
                var fragments = new[] { wrapper.Element("summary")?.Value, wrapper.Element("remarks")?.Value }.Select(NormalizeWhitespace)
                    .Where(fragment => !string.IsNullOrWhiteSpace(fragment))
                    .Cast<string>()
                    .ToArray();

                return fragments.Length == 0 ? null : string.Join(" ", fragments);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string? NormalizeWhitespace(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var normalized = Regex.Replace(value, @"\s+", " ", RegexOptions.CultureInvariant).Trim();
            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }
    }
}
