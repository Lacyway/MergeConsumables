using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Request;
using System.Text.Json.Serialization;

namespace MergeConsumables;

public record CombineItemsModel : BaseInteractionRequestData
{
    [JsonPropertyName("sourceItem")]
    public MongoId? SourceItem { get; set; }

    [JsonPropertyName("targetItem")]
    public MongoId? TargetItem { get; set; }

    [JsonPropertyName("sourceAmount")]
    public float SourceAmount { get; set; }

    [JsonPropertyName("targetAmount")]
    public float TargetAmount { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
