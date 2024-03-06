using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons;
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
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += TF2.Round(weapon.maxAmmoReserve * 0.2f);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += TF2.Round(weapon.maxAmmoClip * 0.2f);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += 40;
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

    public class SmallAmmoPotion : ModItem
    {
        public override string Texture => "TF2/Content/Items/Consumables/SmallAmmoBox";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ItemRarityID.White;
        }

        public override bool? UseItem(Player player)
        {
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += TF2.Round(weapon.maxAmmoReserve * 0.2f);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += TF2.Round(weapon.maxAmmoClip * 0.2f);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += 40;
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
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += TF2.Round(weapon.maxAmmoReserve * 0.5f);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += TF2.Round(weapon.maxAmmoClip * 0.5f);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += 100;
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

    public class MediumAmmoPotion : ModItem
    {
        public override string Texture => "TF2/Content/Items/Consumables/MediumAmmoBox";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 2);
            Item.rare = ItemRarityID.White;
        }

        public override bool? UseItem(Player player)
        {
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += TF2.Round(weapon.maxAmmoReserve * 0.5f);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += TF2.Round(weapon.maxAmmoClip * 0.5f);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += 100;
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
            Item.rare = ItemRarityID.White;
        }

        public override bool ItemSpace(Player player) => true;

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), player.Center);
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += TF2.Round(weapon.maxAmmoReserve);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += TF2.Round(weapon.maxAmmoClip);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += 200;
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

    public class LargeAmmoPotion : ModItem
    {
        public override string Texture => "TF2/Content/Items/Consumables/LargeAmmoBox";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 10;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useTurn = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup");
            Item.consumable = true;
            Item.maxStack = 30;
            Item.potion = true;
            Item.value = Item.buyPrice(platinum: 5);
            Item.rare = ItemRarityID.White;
        }

        public override bool? UseItem(Player player)
        {
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += TF2.Round(weapon.maxAmmoReserve);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += TF2.Round(weapon.maxAmmoClip);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
            if (player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer)
                player.GetModPlayer<TF2Player>().metal += 200;
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