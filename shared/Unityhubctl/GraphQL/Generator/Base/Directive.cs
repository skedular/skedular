namespace Unityhubctl.GraphQL.Generator.Base;

public enum DirectiveType
{
    Constraint,
    Authorize
}

public class Directive(IList<DirectiveArgument>? arguments, DirectiveType directiveType)
{
    public override string ToString() =>
        directiveType switch
        {
            DirectiveType.Constraint => ConvertConstraint(),
            DirectiveType.Authorize => ConvertAuthorize(),
            _ => string.Empty
        };

    private string ConvertConstraint()
    {
        if (arguments == null)
        {
            return string.Empty;
        }

        var spacer = $"{Environment.NewLine}            ";
        var formattedDirectiveArguments = arguments
            .Select(argument => argument.ToString(ArgumentSeparatorType.Paraneteses)).Select(s => $"[{s}]");

        return string.Join(spacer, formattedDirectiveArguments) + spacer;
    }

    private string ConvertAuthorize()
    {
        var spacer = $"{Environment.NewLine}            ";
        var formattedDirectiveArguments = arguments is null
            ? []
            : arguments.Count == 0
                ? ["[Authorize]"]
                : arguments
                    .Select(argument => argument.ToString(ArgumentSeparatorType.EqualSign))
                    .Select(s => $"[Authorize({s})]");

        return string.Join(spacer, formattedDirectiveArguments) + spacer;
    }
}
