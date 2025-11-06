using Diz.LanguageExtensions;
using EFT.InventoryLogic;

namespace MergeConsumables.Errors;

public class WrongTypeError(Item item) : Error
{
    public Item Item = item;

    public override string ToString()
    {
        return $"Invalid item type: {Item.GetType().Name}";
    }
}
