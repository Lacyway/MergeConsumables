using Newtonsoft.Json;
using System;

namespace MergeConsumables.Models;

[Serializable]
public class CombineItemsModel(string sourceItem, string targetItem, float sourceAmount, float targetAmount, string type) : GClass3471
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

    public override bool Queued
    {
        get
        {
            return false;
        }
    }
}
