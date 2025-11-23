using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using SPTarkov.Server.Core.Utils.Json.Converters;

namespace MergeConsumables;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.lacyway.mc";
    public override string Name { get; init; } = "MergeConsumablesServer";
    public override string Author { get; init; } = "Lacyway";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.2");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "CC BY-NC-ND 4.0";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 6000)]
public class MergeConsumables(ISptLogger<MergeConsumables> logger, JsonUtil jsonUtil) : IOnLoad
{
    public const string CombineRouter = "Combine";

    public Task OnLoad()
    {
        BaseInteractionRequestDataConverter.RegisterModDataHandler(CombineRouter, jsonUtil.Deserialize<CombineItemsModel>);

        logger.Success("MergeConsumables loaded!");

        return Task.CompletedTask;
    }
}
