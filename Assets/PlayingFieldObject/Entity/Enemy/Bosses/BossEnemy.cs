using UnityEngine;

public abstract class BossEnemy : Enemy
{
  [SerializeField] private string name;
  private protected override void Start()
  {
    base.Start();
    WeaponUiManager.main.BossLock(name);
  }

  private protected override void Die(){
    base.Die();
    WeaponUiManager.main.BossUnlock();
  }
}
