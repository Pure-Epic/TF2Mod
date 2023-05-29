using Terraria.GameContent.ItemDropRules;

namespace TF2.Content.ItemDrops
{
    // Very simple drop condition: drop during daytime
    public class TFDropCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Drops from defeat";
        }
    }
}