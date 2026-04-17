namespace SharedService.SharedKernel.Messaging.Files.Events;

public sealed record FileUploaded(
    Guid AssetId,
    string FileName,
    string ContentType, // mime в будущем для frontEnd "video/mp4"
    long Size,
    string AssetType, // VIDEO, PHOTO, PREVIEW
    Guid TargetEntityId, // department.id
    string TargetEntityType); // department, location, position