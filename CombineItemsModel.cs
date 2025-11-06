using Newtonsoft.Json;
using System;

namespace MergeConsumables;

[Serializable]
public class CombineItemsModel(string sourceItem, string targetItem, float sourceAmount, float targetAmount, string type)
{
    public string Action = "Combine";

    [JsonProperty("sourceItem")]
    public string SourceItem = sourceItem;

    [JsonProperty("targetItem")]
    public string TargetItem = targetItem;

    [JsonProperty("sourceAmount")]
    public float SourceAmount = sourceAmount;

    [JsonProperty("targetAmount")]
    public float TargetAmount = targetAmount;

    [JsonProperty("type")]
    public string Type = type;
}
