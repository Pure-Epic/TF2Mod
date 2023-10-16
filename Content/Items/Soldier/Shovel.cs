namespace TF2.Content.Items.Soldier
{
    public class Shovel : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Melee, Stock, Starter);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/shovel_swing");
        }
    }
}