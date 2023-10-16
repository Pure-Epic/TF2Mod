using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Demoman
{
    public class CharginTarge : TF2AccessorySecondary
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chargin' Targe");
            Tooltip.SetDefault("Demoman's Unlocked Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+50% contact damage resistance on wearer\n"
                + "+30% projectile damage resistance on wearer")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "Keybind: Charge toward your enemies and remove debuffs.\n"
                + "Gain a critical melee strike after impacting an enemy at distance.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.shield = true;
            p.shieldTimer = 720;
            p.shieldType = 1;
            player.noKnockback = true;

            ShieldPlayer s = player.GetModPlayer<ShieldPlayer>();
            if (s.chargeActive && !s.chargeProjectileCreated)
            {
                Vector2 chargeDirection = player.DirectionTo(Main.MouseWorld);
                float speed = 25f;
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, chargeDirection * 2.5f, ModContent.ProjectileType<Projectiles.Demoman.ShieldHitbox>(), (int)((50f + player.GetModPlayer<EyelanderPlayer>().heads) * p.classMultiplier), 0f, player.whoAmI);
                player.velocity = chargeDirection * speed;
                s.chargeProjectileCreated = true;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = player.buffType[i];
                    if (Main.debuff[buffTypes] && player.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !Buffs.TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        player.DelBuff(i);
                        i = -1;
                    }
                }
            }
        }
    }

    public class ShieldPlayer : ModPlayer
    {
        public int timer;
        public bool chargeActive;
        public bool chargeProjectileCreated;

        public override void ResetEffects()
        {
            if (!Player.GetModPlayer<TF2Player>().shield)
                timer = 0;
        }

        public override void PostUpdate()
        {
            timer = Utils.Clamp(timer, 0, Player.GetModPlayer<TF2Player>().shieldTimer);
            if (Player.GetModPlayer<TF2Player>().shield && !chargeActive)
                timer++;
        }


        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.ShieldCharge.JustPressed && timer >= Player.GetModPlayer<TF2Player>().shieldTimer)
            {
                chargeActive = true;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/demo_charge_windup1"), Player.Center);
                timer = 0;
            }
        }
    }

    public class CharginTargePlayer : ShieldPlayer
    {
        public int buffDelay;
        public bool activateGracePeriod;

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (Player.GetModPlayer<TF2Player>().shield && Player.GetModPlayer<TF2Player>().shieldType == 1)
                damage /= 5;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (Player.GetModPlayer<TF2Player>().shield && Player.GetModPlayer<TF2Player>().shieldType == 1)
                damage = (int)(damage / 1.5f);
        }

        public override void PostUpdate()
        {
            if (!activateGracePeriod) return;
            buffDelay++;
            if (buffDelay >= 30)
            {
                Player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
                buffDelay = 0;
            }
        }
    }
}