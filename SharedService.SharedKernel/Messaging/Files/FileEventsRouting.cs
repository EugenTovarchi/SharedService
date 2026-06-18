namespace SharedService.SharedKernel.Messaging.Files;

public static class FileEventsRouting
{
    public const string EXCHANGE = "file.events";
    public const string ALL_DIRECTORY_EVENTS = "directory.*.*";
    public const string ALL_FILE_UPLOADED = "file.uploaded.#";
    public const string ALL_FILE_DELETED = "file.deleted.#";

    public const string DEPARTMENT_FILE_UPLOADED = "file.uploaded.*.department";
    public const string DEPARTMENT_FILE_DELETED = "file.deleted.*.department";

    public const string LOCATION_FILE_UPLOADED = "file.uploaded.*.location";
    public const string LOCATION_FILE_DELETED = "file.deleted.*.location";

    public const string POSITION_FILE_UPLOADED = "file.uploaded.*.position";
    public const string POSITION_FILE_DELETED = "file.deleted.*.position";

    public static class RoutingKeys
    {
        // bindingKey: file.deleted.photo.department -> to queue: directory.file.events
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