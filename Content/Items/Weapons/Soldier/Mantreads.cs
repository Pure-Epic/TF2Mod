using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Content.Tiles.Crafting;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Mounts;
using Terraria.DataStructures;
using TF2.Content.Dusts;
using Terraria.Audio;
using TF2.Content.NPCs.Enemies;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class Mantreads : TF2Accessory
    {
        protected override string LegTexture => "TF2/Content/Textures/Items/Soldier/Mantreads";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 2, refined: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description) => AddPositiveAttribute(description);

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<MantreadsPlayer>().mantreadsEquipped;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MantreadsPlayer>().mantreadsEquipped = true;
            player.noFallDmg = true;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<Gunboats>().AddIngredient<RefinedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class MantreadsPlayer : ModPlayer
    {
        public bool mantreadsEquipped;

        public override void ResetEffects() => mantreadsEquipped = false;

        public override void PostUpdate()
        {
            if (mantreadsEquipped)
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (Player.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Main.myPlayer && Player.velocity.Y > 0)
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(TF2.TF2DeathMessagesLocalization[3].Format(player.name, Player.name)), TF2.Round((10 + TF2.Round((Player.position.Y / 16f - Player.fallStart) * Player.gravDir - 25 + Player.extraFall) * 30) * Player.GetModPlayer<TF2Player>().damageMultiplier), player.direction, true, knockback: 10f, scalingArmorPenetration: 1f);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/mantreads"), player.Center);
                        Dust.NewDust(player.Center, 0, 0, ModContent.DustType<Stomp>(), Scale: 1f);
                        break;
                    }
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (Player.Hitbox.Intersects(npc.Hitbox) && npc.ModNPC is BLUMercenary && Player.velocity.Y > 0)
                    {
                        if (npc.immune[Player.whoAmI] > 0) return;
                        int damage = TF2.Round((Player.position.Y / 16f - Player.fallStart) * Player.gravDir - 25 - Player.extraFall) * 30;
                        TF2.Minimum(ref damage, 0);
                        Player.GetModPlayer<TF2Player>().HitNPC(npc, TF2.Round((10 + damage) * Player.GetModPlayer<TF2Player>().damageMultiplier), 10f, Player.direction);
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/mantreads"), npc.Center);
                        Dust.NewDust(npc.Center, 0, 0, ModContent.DustType<Stomp>(), Scale: 1f);
                        npc.immune[Player.whoAmI] += TF2.Time(0.2);
                        break;
                    }
                }
            }
        }

        public override void UpdateEquips()
        {
            if (mantreadsEquipped)
                TF2Player.SetPlayerSpeed(Player, (Player.velocity.Y != 0 && !Player.HasBuff<TF2MountBuff>()) ? 200 : 125);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (mantreadsEquipped)
                modifiers.Knockback *= 0.25f;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            int damage = TF2.Round((Player.position.Y / 16f - Player.fallStart) * Player.gravDir - 25 - Player.extraFall) * 30;
            TF2.Minimum(ref damage, 0);
            if (mantreadsEquipped && Player.velocity.Y >= 5)
            {
                Player.GetModPlayer<TF2Player>().HitNPC(npc, TF2.Round((10 + damage) * Player.GetModPlayer<TF2Player>().damageMultiplier), 10f, modifiers.HitDirection);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/mantreads"), npc.Center);
                Dust.NewDust(npc.Center, 0, 0, ModContent.DustType<Stomp>(), Scale: 1f);
            }
        }
    }
}