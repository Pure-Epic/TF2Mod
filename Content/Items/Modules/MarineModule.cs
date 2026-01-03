using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Modules
{
    public class MarineModule : TF2Module
    {
        public override bool Passive => true;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Purchase;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            AddPositiveAttribute(tooltips);
            AddNeutralAttribute(tooltips);
        }

        protected override void ModuleUpdate(Player player)
        {
            player.GetModPlayer<MarineModulePlayer>().marineModuleEquipped = true;
            player.buffImmune[BuffID.Blackout] = player.buffImmune[BuffID.Bleeding] = player.buffImmune[BuffID.Burning] = player.buffImmune[BuffID.Chilled] = player.buffImmune[BuffID.Confused] = player.buffImmune[BuffID.Cursed] = player.buffImmune[BuffID.CursedInferno] = player.buffImmune[BuffID.Darkness] = player.buffImmune[BuffID.Frostburn] = player.buffImmune[BuffID.Frozen] = player.buffImmune[BuffID.Electrified] = player.buffImmune[BuffID.MoonLeech] = player.buffImmune[BuffID.Obstructed] = player.buffImmune[BuffID.OnFire] = player.buffImmune[BuffID.Poisoned] = player.buffImmune[BuffID.Silenced] = player.buffImmune[BuffID.Slow] = player.buffImmune[BuffID.Stoned] = player.buffImmune[BuffID.Venom] = player.buffImmune[BuffID.VortexDebuff] = player.buffImmune[BuffID.Weak] = player.buffImmune[BuffID.Webbed] = true;
            player.waterWalk = true;
            player.waterWalk2 = true;
            player.breathCD = 0;
            player.ignoreWater = true;
            player.canFloatInWater = true;
            player.lavaImmune = true;
            player.shimmerImmune = true;
        }
    }

    public class MarineModulePlayer : ModPlayer
    {
        public bool marineModuleEquipped;

        public override void ResetEffects() => marineModuleEquipped = false;
    }

    public class MarineModuleMount : ModMount
    {
        public override string Texture => "TF2/Content/Textures/Items/Modules/MarineModuleMinecart";

        public override void SetStaticDefaults()
        {
            MountID.Sets.Cart[Type] = true;
            MountID.Sets.FacePlayersVelocity[Type] = true;
            Mount.SetAsMinecart(MountData, ModContent.BuffType<MarineModuleBuff>(), MountData.frontTexture);
            MountData.delegations.MinecartDust = DelegateMethods.Minecart.Sparks;
            MountData.delegations.MinecartLandingSound = DelegateMethods.Minecart.LandingSound;
            MountData.delegations.MinecartBumperSound = DelegateMethods.Minecart.BumperSound;
            MountData.runSpeed = MountData.MinecartUpgradeRunSpeed;
            MountData.acceleration = MountData.MinecartUpgradeAcceleration;
            MountData.jumpSpeed = MountData.MinecartUpgradeJumpSpeed;
            MountData.dashSpeed = MountData.MinecartUpgradeDashSpeed;
            MountData.jumpHeight = MountData.MinecartUpgradeJumpHeight;
        }
    }

    public class MarineModuleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<MarineModuleMount>(), player);
            player.buffTime[buffIndex] = TF2.Time(1);
        }
    }
}