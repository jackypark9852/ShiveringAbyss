using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class JellyFishEnemy : Enemy
{
    // ������Ѳ��״̬�»����� waypoints ��������Ŀ��ص�
    // ������Unity ������ѡ����Ϸ������Ϊ waypoint
    // �ز����� waypoint prefab��������Ϊ���� waypoint ����Ϸ����


    [Header("�ƶ�AI")]
    [SerializeField] Transform moveDestination;

    [Header("Ѳ��AI")]
    [SerializeField] Transform waypoint1; // Ѳ�ߵ�1
    [SerializeField] Transform waypoint2; // Ѳ�ߵ�2
    [SerializeField] float waypointReachThreshold; // �ж����ﵽ�� waypoint
    [SerializeField] float newPatrolDestinationDelay; // �׳浽��Ŀ�ĵغ�ǰ���µ�Ŀ���֮ǰ���ӳ�ʱ��

    [Header("׷ɱ��Χ")]
    [SerializeField] Transform chaseRadiusCenter; //׷ɱ��Χ���ĵ�
    [SerializeField] float chaseRadius; //׷ɱ��Χ�뾶


    [SerializeField] float knockBackHeightMultiplier; // ����Ч���Ĵ�ֱ���������� ����Խ�ߣ�����ͷɵ�Խ��

    [SerializeField] float unchaseThreshhold;

    [SerializeField] LayerMask playerLayerMask; 


    protected bool isMovementDisabled; // ��ֵΪ��ʵʱ�������޷��ƶ�

    bool isHostile = false; //�Ƿ���׷ɱ״̬

    bool ranDeathSequence = false; //�Ƿ񴦷���������

    protected override void Start()
    {
        base.Start();
        moveDestination.position = GetNewPatrolDestination(); //ָ����ʼ����Ŀ��ص�
    }

    protected override void Update()
    {
        base.Update(); // Checks for death

        if (!isMovementDisabled && !isDead)
        {
            DetectPlayer();
            if (isHostile)
            { //�������׷ɱ״̬
                moveDestination.position = playerMovement.transform.position;
                if(Vector3.Magnitude(moveDestination.position - transform.position) > unchaseThreshhold) 
                    isHostile = false; 
            }
            else
            { //�����������Ѳ��״̬
                // �����������Ŀ��ص�
                if (Mathf.Abs(transform.position.x - moveDestination.position.x) < waypointReachThreshold)
                {
                    //ֹͣ����
                    DisableMovement(newPatrolDestinationDelay);
                    //ָ���µ�Ŀ�궨��
                    moveDestination.position = GetNewPatrolDestination();
                    return; //��ǰ�����ս�Update(), ��һ֡�����ٽ����ƶ�
                }
            }
            Move();
        }
        if(isDead && !ranDeathSequence) {
            ranDeathSequence = true;
            myAnimator.Play("DeadAnimation"); 
            myRigidBody2D.gravityScale = 5f;
        }
    }

    private Vector2 GetNewPatrolDestination()
    {
        // ������waypoint ֮��Ѱ��һ�������ͬ��yֵ�ĵ���Ϊ��Ŀ��ص�
        Vector2 newDest = new Vector2(UnityEngine.Random.Range(waypoint1.position.x, waypoint2.position.x), UnityEngine.Random.Range(waypoint1.position.y, waypoint2.position.y));
        // Debug.Log(newDest);
        return newDest;
    }

    protected override void Move()
    { // ��Ŀ��� moveDestination �ƶ�
        myAnimator.Play("MoveAnimation");
        // Debug.Log("Jelly Moved");
        float xDir = (moveDestination.position.x - transform.position.x);
        float yDir = (moveDestination.position.y - transform.position.y);
        Vector3 moveDirection = Vector3.Normalize(new Vector3(xDir, yDir, 0));


        myRigidBody2D.velocity = moveDirection*moveSpeed;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        KnockBack(playerMovement.getKnockbackForce(), playerMovement.transform); // ����� playerMovement�ű��еõ�����ǿ�ȣ� λ����Ϣ
        isHostile = true; //����
    }

    protected virtual void KnockBack(float knockBackForce, Transform damageSourcePosition)
    { // ����ǿ�Ⱥ��˺���Դλ�ã��������˾���ͷ���
        myRigidBody2D.velocity = Vector2.zero;
        float dir = Mathf.Sign(transform.position.x - damageSourcePosition.position.x);
        DisableMovement(playerMovement.getKnockbackDuration()); // �����ڻ���״̬���ܿ��ƣ��޷��ƶ�
        myRigidBody2D.AddForce((Vector2.right * dir + Vector2.up * 2) * knockBackForce);
    }
    void DetectPlayer() { // Ѱ����췶Χ�ڵ����
        // �Թ���Ϊ���ģ������췶Χ playerDetectionRadius �ڵ������ײ��
        Collider2D playerColliderInRadius = Physics2D.OverlapCircle(chaseRadiusCenter.position, chaseRadius, playerLayerMask); 
        if(playerColliderInRadius != null) { // �緶Χ������ң�
            isHostile = true; 
        }
        else {
            isHostile = false; 
        }

    }
    protected void DisableMovement(float duration)
    { // ���ﴦ�ڿ���״̬�£� �޷��ƶ�
        myAnimator.Play("StayAnimation");
        isMovementDisabled = true;
        Invoke("EnableMovement", duration); // ��ʱ�������
    }
    protected void EnableMovement()
    {
        isMovementDisabled = false;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // ����ԴȪ��ʾ���ﵽ��waypoint���ж���Χ
        Gizmos.DrawWireSphere(transform.position, waypointReachThreshold);

        Gizmos.color = Color.green; // �����׳�����Ŀ��ص�
        Gizmos.DrawWireSphere(moveDestination.position, .3f);

        Gizmos.color = Color.red; //����׷ɱ��Χ
        Gizmos.DrawWireSphere(chaseRadiusCenter.position, chaseRadius);
    }
}
