using UnityEngine;
using DanmakU;
using System.Collections.Generic;
using System;

public partial class Enemy : DanmakuCollider
{
    [SerializeField]
    private List<AttackBehavior> attackBehaviors;
    [SerializeField]
    private List<MovementBehavior> movementBehaviors;

    private AttackBehavior attackBehavior;
    private MovementBehavior movementBehavior;

    private void InitializeAttackBehavior()
    {
        if(attackBehaviors.Count == 0)
            return;

        attackBehavior = (AttackBehavior)Instantiate(attackBehaviors[0], Vector3.zero, Quaternion.identity);
        attackBehavior.transform.parent = transform.parent;
        attackBehavior.BehaviorStart(this);
        attackBehaviors.RemoveAt(0);
    }

    private void InitializeMovementBehavior()
    {
        if(movementBehaviors.Count == 0)
            return;
        
        movementBehavior = (MovementBehavior)Instantiate(movementBehaviors[0], Vector3.zero, Quaternion.identity);
        movementBehavior.transform.parent = transform.parent;
        movementBehavior.BehaviorStart(this);
        movementBehaviors.RemoveAt(0);
    }

    public abstract class AttackBehavior : MonoBehaviour
    {
        public float duration = -1;
        public bool repeat = false;

        protected Player player;
        protected float time = 0;

        public virtual void BehaviorStart(Enemy enemy)
        {
            player = LevelController.Singleton.Player;
            if(duration < 0)
                duration = float.MaxValue;

            if(repeat)
                enemy.attackBehaviors.Add(this);
        }

        public virtual void BehaviorUpdate(Enemy enemy)
        {
            time += Time.deltaTime;
            if(time > duration)
                BehaviorEnd(enemy);
        }

        public virtual void BehaviorFixedUpdate(Enemy enemy) { }

        public virtual void BehaviorEnd(Enemy enemy)
        {
            if(!enemy.attackBehavior.Equals(this))
                return;

            if(enemy.attackBehaviors.Count > 0)
                enemy.InitializeAttackBehavior();
            else
                enemy.Die();
        }
    }

    public abstract class MovementBehavior : MonoBehaviour
    {
        public float duration = -1;
        public bool repeat = false;

        protected Player player;
        protected float time = 0;

        public virtual void BehaviorStart(Enemy enemy)
        {
            player = LevelController.Singleton.Player;
            if(duration < 0)
                duration = float.MaxValue;

            if(repeat)
                enemy.movementBehaviors.Add(this);
        }

        public virtual void BehaviorUpdate(Enemy enemy)
        {
            time += Time.deltaTime;
            if(time > duration)
                BehaviorEnd(enemy);
        }

        public virtual void BehaviorFixedUpdate(Enemy enemy) { }

        public virtual void BehaviorEnd(Enemy enemy)
        {
            if(!enemy.movementBehavior.Equals(this))
                return;
            
            if(enemy.movementBehaviors.Count > 0)
                enemy.InitializeMovementBehavior();
            else
                enemy.Die();
        }
    }
}
