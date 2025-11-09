using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Spy;

namespace TF2.Content.Items.Consumables
{
    public class SmallAmmoBox : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            TF2.AddAmmo(player, 20);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.HasShield && player.GetModPlayer<PersianPersuaderPlayer>().persianPersuaderEquipped)
            {
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                shield.timer += TF2.Round(shield.ShieldRechargeTime * 0.2f);
            }
            if (p.currentClass == TF2Item.Engineer)
                p.metal += 40;
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = TF2.Time(10);
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += TF2.Time(6.5);
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = TF2.Time(14);
            Item.TurnToAir();
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class SmallAmmoPotion : TF2Item
    {
        public override string Texture => "TF2/Content/Items/Consumables/SmallAmmoBox";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.useTime = Item.useAnimation = TF2.Time(1);
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 1);
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool? UseItem(Player player)
        {
            TF2.AddAmmo(player, 20);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.HasShield && player.GetModPlayer<PersianPersuaderPlayer>().persianPersuaderEquipped)
            {
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                shield.timer += TF2.Round(shield.ShieldRechargeTime * 0.2f);
            }
            if (p.currentClass == Engineer)
                p.metal += 40;
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = TF2.Time(10);
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += TF2.Time(6.5);
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = TF2.Time(14);
            return true;
        }
    }

    public class MediumAmmoBox : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            TF2.AddAmmo(player, 50);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.HasShield && player.GetModPlayer<PersianPersuaderPlayer>().persianPersuaderEquipped)
            {
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                shield.timer += TF2.Round(shield.ShieldRechargeTime * 0.5f);
            }
            if (p.currentClass == TF2Item.Engineer)
                p.metal += 100;
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = TF2.Time(10);
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += TF2.Time(6.5);
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = TF2.Time(14);
            Item.TurnToAir();
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class MediumAmmoPotion : TF2Item
    {
        public override string Texture => "TF2/Content/Items/Consumables/MediumAmmoBox";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.useTime = Item.useAnimation = TF2.Time(1);
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 2);
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool? UseItem(Player player)
        {
            TF2.AddAmmo(player, 50);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.HasShield && player.GetModPlayer<PersianPersuaderPlayer>().persianPersuaderEquipped)
            {
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                shield.timer += TF2.Round(shield.ShieldRechargeTime * 0.5f);
            }
            if (p.currentClass == Engineer)
                p.metal += 100;
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = TF2.Time(10);
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += TF2.Time(6.5);
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = TF2.Time(14);
            return true;
        }
    }

    public class LargeAmmoBox : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 0;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 50;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            TF2.AddAmmo(player, 100);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.HasShield && player.GetModPlayer<PersianPersuaderPlayer>().persianPersuaderEquipped)
            {
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                shield.timer += TF2.Round(shield.ShieldRechargeTime);
            }
            if (p.currentClass == TF2Item.Engineer)
                p.metal += 200;
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = TF2.Time(10);
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += TF2.Time(6.5);
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = TF2.Time(14);
            Item.TurnToAir();
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public class LargeAmmoPotion : TF2Item
    {
        public override string Texture => "TF2/Content/Items/Consumables/LargeAmmoBox";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.useTime = Item.useAnimation = TF2.Time(1);
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 5);
            WeaponAddQuality(Unique);
            noThe = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltips(tooltips);

        public override bool? UseItem(Player player)
        {
            TF2.AddAmmo(player, 100);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.HasShield && player.GetModPlayer<PersianPersuaderPlayer>().persianPersuaderEquipped)
            {
                ShieldPlayer shield = ShieldPlayer.GetShield(player);
                shield.timer += TF2.Round(shield.ShieldRechargeTime);
            }
            if (p.currentClass == Engineer)
                p.metal += 200;
            if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                player.GetModPlayer<CloakPlayer>().cloakMeter = TF2.Time(10);
            if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += TF2.Time(6.5);
            if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                player.GetModPlayer<FeignDeathPlayer>().cloakMeter = TF2.Time(14);
            return true;
        }
    }
}