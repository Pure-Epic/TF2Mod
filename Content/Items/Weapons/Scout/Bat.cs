namespace TF2.Content.Items.Weapons.Scout
{
    public class Bat : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Stock, Starter);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
        }
    }
}