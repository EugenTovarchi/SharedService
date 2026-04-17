namespace SharedService.SharedKernel.Messaging.Files.Events;

public sealed record FileDeleted(
    Guid AssetId,
    string AssetType,
    Guid TargetEntityId,
    string TargetEntityType);