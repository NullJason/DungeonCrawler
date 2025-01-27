using UnityEngine;
using System.Collections.Generic;

public class Superslime : BossEnemy
{
  [SerializeField] private protected List<GameObject> minions;
  [SerializeField] private float initialSpeed;
  [SerializeField] private float finalSpeed;
  [SerializeField] private int reload = -1; //Leave as neagative to use the Weapon's reload time instead!
  [SerializeField] private float maxChaseDistance;
  [SerializeField] private int healthDropoff; //If health is greater than healthDropoff, GetSpeed() will return initialSpeed. If health is 0, GetSpeed() would return finalSpeed, if you somehow managed to call it. If health is somewhere in between, GetSpeed() will return somewhere in between. TL,DR- as health goes below healthDropoff, speed goes faster.

  private protected override void OnHit(){
    GameObject toBeSummoned = minions[Random.Range(0, minions.Count)];
    if(toBeSummoned != null) Instantiate(toBeSummoned, RandomPosition(1.0f), transform.rotation, transform.parent);
    Debug.Log("Speed: " + GetSpeed());
  }

  private protected override EnemyStateMachine GetStateMachine(){
    EnemyState charge = new EnemyState(delegate(){

        transform.Translate(TowardsPlayer() * GetSpeed());
        Debug.Log("Trying to charge!");
      });
    EnemyState attack = new EnemyState(delegate(){ Attack(TowardsPlayer()); Debug.Log("Trying to attack!"); });
    EnemyState idle = new EnemyState(delegate(){ Debug.Log("Doing naught!"); });
    EnemyStateTransition startAttacking = new EnemyStateTransition(delegate(){
        int i = reload;
        if(i < 0) i = currentWeapon.ResetTime();
        return TimeOver(i);
    }, attack);
    EnemyStateTransition startIdling = new EnemyStateTransition(delegate()
    {
        return !CloseToPlayer(maxChaseDistance);
    }, idle);
    EnemyStateTransition startCharging = new EnemyStateTransition(delegate()
    {
        return CloseToPlayer(maxChaseDistance);
    }, charge);
    charge.AddTransition(startAttacking);
    attack.AddTransition(startIdling);
    attack.AddTransition(startCharging);
    idle.AddTransition(startCharging);
    return new EnemyStateMachine(idle);
  }

  private protected float GetSpeed(){
    if(GetHealth() > healthDropoff) return initialSpeed;
    return (float)initialSpeed + ((float)finalSpeed - (float)initialSpeed) * (1.00f - ((float)GetHealth() / (float)healthDropoff)); //I wasn't sure which number was being incorrectly treated as an int, so I just made sure all of them were floats.
  }
  private protected Vector3 RandomPosition(float range){
    return new Vector3(transform.position.x + Random.Range(-range, range), transform.position.y + Random.Range(-range, range), transform.position.z);
  }
}
