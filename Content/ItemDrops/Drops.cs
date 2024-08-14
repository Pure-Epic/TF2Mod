using Terraria.GameContent.ItemDropRules;

namespace TF2.Content.ItemDrops
{
    // Very simple drop condition: drop during daytime
    public class TFDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info) => true;

        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => "Drops from defeat";
    }
}