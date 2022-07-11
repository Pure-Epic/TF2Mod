using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.Localization;

namespace TF2.Items.Currencies
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