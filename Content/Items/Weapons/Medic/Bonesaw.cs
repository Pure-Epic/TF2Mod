namespace TF2.Content.Items.Weapons.Medic
{
    public class Bonesaw : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Stock, Starter);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
        }
    }
}