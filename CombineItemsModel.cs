using Newtonsoft.Json;
using System;

namespace MergeConsumables
{
	[Serializable]
	public class CombineItemsModel
	{
		public string Action = "Combine";

		[JsonProperty("sourceItem")]
		public string SourceItem;

		[JsonProperty("targetItem")]
		public string TargetItem;

		[JsonProperty("sourceAmount")]
		public float SourceAmount;

		[JsonProperty("targetAmount")]
		public float TargetAmount;

		[JsonProperty("type")]
		public string Type;

		public CombineItemsModel(string sourceItem, string targetItem, float sourceAmount, float targetAmount, string type)
		{
			SourceItem = sourceItem;
			TargetItem = targetItem;
			SourceAmount = sourceAmount;
			TargetAmount = targetAmount;
			Type = type;
		}
	}
}
