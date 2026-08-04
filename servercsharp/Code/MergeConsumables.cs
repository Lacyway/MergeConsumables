using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace MergeConsumables;

public sealed record MyMdMetaData : IModMetadata
{
    public string ModGuid { get; init; } = "com.lacyway.mc";
    public string Name { get; init; } = "MergeConsumablesServer";
    public string Author { get; init; } = "Lacyway";
    public List<string>? Contributors { get; init; } = ["Lacyway", "tyfon"];
    public SemanticVersioning.Version Version { get; init; } = new("1.6.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new(">=4.1.1");
    public bool HasPrepatcher { get; init; }
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; } = "https://github.com/Lacyway/MergeConsumables";
    public string License { get; init; } = "CC BY-NC-ND 4.0";
}

[Injectable(TypePriority = OnLoadOrder.PostLoad)]
public sealed class MergeConsumables(ISptLogger<MergeConsumables> logger) : IOnLoad
{
    internal const string CombineRouter = "Combine";

    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        logger.Success("MergeConsumables loaded!");
        return Task.CompletedTask;
    }
}
