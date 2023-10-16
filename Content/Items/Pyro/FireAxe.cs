namespace TF2.Content.Items.Pyro
{
    public class FireAxe : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Pyro, Melee, Stock, Starter);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
        }
    }
}