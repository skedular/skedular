namespace Enterprise.Shared.Models;

public record Edge<T>(string Cursor, T Node);
