using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;


public class TrackingMechanism : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public LayerMask layerMask;

    
    public Transform player;
    
    public int walkCount;
    private int currentWalkCount;

    public float speed;

    public float chaseRange;

    public float stopRange;

    private Vector3 vector;

    private bool isSlowed = false;
    public float slowDuration; // �ӵ� ���� ���� �ð�
    public float slowedSpeed; // ������ �ӵ�



    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        StartCoroutine(MoveCoroutine());

    }

    IEnumerator MoveCoroutine()
    {
            while (true) 
            {
                // �÷��̾�� �� ������ �Ÿ� ���
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // 1. �÷��̾ ���� �Ÿ� �ȿ� �ִ��� Ȯ��
                if (distanceToPlayer <= chaseRange)
                {
                    // 2. �÷��̾ ���� �Ÿ� �ٱ��� �ִ��� Ȯ��
                    if (distanceToPlayer > stopRange)
                    {

                        // 3. �÷��̾� �������� ���� �̵�
                        Vector3 direction = (player.position - transform.position).normalized; // ���� ���

                        // ������ X�� �Ǵ� Y�����θ� �����̰� ����
                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            // X�� �������θ� �̵�
                            transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0, 0);
                        }
                        else
                        {
                            // Y�� �������θ� �̵�
                            transform.position += new Vector3(0, direction.y * speed * Time.deltaTime, 0);
                        }
                    }
                }
              
                yield return null;

            }
    }

    public void SlowDown()
    {
        if (!isSlowed)
        {
            isSlowed = true;
            speed = 0;  // �ӵ� ����
            StartCoroutine(ResetSpeedAfterDelay());
        }
    }

    // ���� �ð� �� �ӵ��� ������� �ǵ���
    IEnumerator ResetSpeedAfterDelay()
    {
        yield return new WaitForSeconds(slowDuration); // slowDuration �ð� ���
        speed = 50.0f; // ���� �ӵ��� ����
        isSlowed = false; // �ٽ� �ӵ� ���� ���� �ƴϰ� ����
    }

    IEnumerator WalkCoroutine()
    {
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * speed, 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * speed, 0);
                }

                yield return new WaitForSeconds(0.005f);
            }
            currentWalkCount = 0;
       
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopRange)
        {
            UnityEngine.Debug.Log("�÷��̾ ���� ������ �����߽��ϴ�!");
            Time.timeScale = 0;
            speed = 0;
            stopRange = 0.0f;

        }

        RaycastHit2D hit;
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, layerMask);
        boxCollider.enabled = true;

        if (hit.transform != null)
            return;

    }

    // ������: ���� �Ÿ��� ���� �Ÿ��� ������ Scene �信�� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        // ���� �Ÿ�(Chase Range)�� ǥ���ϴ� �Ķ��� ��
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // ���� �Ÿ�(Stop Range)�� ǥ���ϴ� ������ ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }

}
