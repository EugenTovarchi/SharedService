namespace SharedService.SharedKernel.Messaging.Files;

public static class FileEventsRouting
{
    public const string EXCHANGE = "file.events";
    public const string ALL_DIRECTORY_EVENTS = "directory.*.*";
    public const string ALL_FILE_UPLOADED = "file.uploaded.#";
    public const string ALL_FILE_DELETED = "file.deleted.#";

    public static class RoutingKeys
    {
        // file.deleted.photo.department (binding to queue: directory.file.events)
        public static string FileDeleted(string assetType, string entityType)
        {
            return $"file.deleted.{assetType.ToLowerInvariant()}.{entityType.ToLowerInvariant()}";
        }

        public static string FileUploaded(string assetType, string entityType)
        {
            return $"file.uploaded.{assetType.ToLowerInvariant()}.{entityType.ToLowerInvariant()}";
        }
    }
}