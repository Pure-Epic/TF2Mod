using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace TF2.Content.Items.Currencies
{
    public class AustraliumCurrency : CustomCurrencySingleCoin
    {
        public AustraliumCurrency(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.Gold;
        }
    }
}