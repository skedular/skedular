namespace Enterprise.Shared.Storage;

public interface IFileStorageService
{
    /// <summary>
    ///     Saves a file to the storage system
    /// </summary>
    /// <param name="fileContent">The file content as byte array</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="subDirectory">Optional subdirectory to organize files</param>
    /// <returns>The path where the file was saved</returns>
    Task<string> SaveFileAsync(byte[] fileContent, string fileName, string contentType, string? subDirectory = null);

    /// <summary>
    ///     Saves a file to the storage system using a stream
    /// </summary>
    /// <param name="fileStream">The file content as stream</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="contentType">MIME type of the file</param>
    /// <param name="subDirectory">Optional subdirectory to organize files</param>
    /// <returns>The path where the file was saved</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, string? subDirectory = null);

    /// <summary>
    ///     Deletes a file from storage
    /// </summary>
    /// <param name="filePath">Path of the file to delete</param>
    Task DeleteFileAsync(string filePath);

    /// <summary>
    ///     Gets the full URL/path for accessing a file
    /// </summary>
    /// <param name="filePath">The stored file path</param>
    /// <returns>Full URL or path to access the file</returns>
    string GetFileUrl(string filePath);

    /// <summary>
    ///     Checks if a file exists
    /// </summary>
    /// <param name="filePath">Path of the file to check</param>
    /// <returns>True if file exists, false otherwise</returns>
    Task<bool> FileExistsAsync(string filePath);

    /// <summary>
    ///     Gets file content as byte array
    /// </summary>
    /// <param name="filePath">Path of the file to read</param>
    /// <returns>File content as byte array</returns>
    Task<byte[]> GetFileAsync(string filePath);

    /// <summary>
    ///     Gets file content as stream
    /// </summary>
    /// <param name="filePath">Path of the file to read</param>
    /// <returns>File content as stream</returns>
    Task<Stream> GetFileStreamAsync(string filePath);
}
