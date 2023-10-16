using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;
using TF2.Content.Items.Spy;

namespace TF2.Content.Items.Ammo
{
    public class PrimaryAmmo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.gunProj[Item.type] = true;
            Item.ResearchUnlockCount = 1000;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 50;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.ammo = Item.type;
            Item.shoot = ProjectileID.Bullet;
            Item.value = Item.buyPrice(copper: 1);
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = 600;
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += 390;
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = 840;
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class SecondaryAmmo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.gunProj[Item.type] = true;
            Item.ResearchUnlockCount = 1000;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.ammo = Item.type;
            Item.shoot = ProjectileID.Bullet;
            Item.value = Item.buyPrice(copper: 1);
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = 600;
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += 390;
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = 840;
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }
}