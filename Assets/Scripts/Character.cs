using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
        BeginDying, //HW1
        Dead, //HW1
    }

    public enum Weapon
    {
        Pistol,
        Bat,
        Fist,
    }

    public Weapon weapon;
    public float runSpeed;
    public float distanceFromEnemy;
    Transform target;
    State state;
    Animator animator;
    Vector3 originalPosition;
    Quaternion originalRotation;
    
    public Character enemy;//HW1 temporary solution:)

    // Start is called before the first frame update
    //Commit from VSCode
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        state = State.Idle;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        target = enemy.gameObject.transform;
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    [ContextMenu("Attack")]
    void AttackEnemy()
    {
        //HW1+++
        if(IsDead())
            return;
        //HW1---
        
        switch (weapon) {
            case Weapon.Bat:
            case Weapon.Fist:
                state = State.RunningToEnemy;
                break;
            case Weapon.Pistol:
                state = State.BeginShoot;
                break;
        }
    }

    string GetParameterNameFromWeaponType(){
        string paramName = "";

        switch(weapon){
            case Weapon.Bat:
                paramName = "MeleeAttack";
                break;
            case Weapon.Fist:
                paramName = "FistAttack";
                break;
        }

        return paramName;
    }

    bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f) {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude) {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    //HW1+++
    public bool IsDead(){
        return state == State.Dead;
    }

    public void GetDamage(){
        state = State.BeginDying;
    }

    public void DamageEnemy(Character currentEnemy){
        if(currentEnemy.IsDead())
            return;
        
        currentEnemy.GetDamage();
    }

    //HW1---

    void FixedUpdate()
    {
        switch (state) {
            case State.Idle:
                animator.SetFloat("Speed", 0.0f);
                transform.rotation = originalRotation;
                break;

            case State.RunningToEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(target.position, distanceFromEnemy))
                    state = State.BeginAttack;
                break;

            case State.BeginAttack:
                //HW1+++
                //not allowed bat waving in air
                if(enemy.IsDead()){
                    SetState(State.RunningFromEnemy);
                    break;
                }
                string paramName = GetParameterNameFromWeaponType();

                if(paramName == ""){
                    Debug.LogError("No such parameter in Animator for this weapon type!");
                    return;
                }

                animator.SetTrigger(paramName);
                //HW1---
                
                //animator.SetTrigger("MeleeAttack");
                state = State.Attack;
                break;

            case State.Attack:
                break;

            case State.BeginShoot:
                animator.SetTrigger("Shoot");
                state = State.Shoot;
                break;

            case State.Shoot:
                break;

            case State.RunningFromEnemy:
                animator.SetFloat("Speed", runSpeed);
                if (RunTowards(originalPosition, 0.0f))
                    state = State.Idle;
                break;
            
            //HW1+++
            case State.BeginDying:
                animator.SetTrigger("Die");
                state = State.Dead;
                break;

            case State.Dead:
                break;
            //HW1---

        }
    }
}
