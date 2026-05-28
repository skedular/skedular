namespace Enterprise.Shared.Email;

public record EmailAttachment(Stream Stream, string Name, string MimeType);
