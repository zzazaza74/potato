using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EnemyDoll : MonoBehaviour
{
    private EnemyManager enemyManager;
    public int Health;

    public float speed; 
    public float range; 
    public float detectionRange; // �÷��̾� ���� ����
    public float returnDelay; // ��� �ð�

    private Vector3 startPosition;
    private GameObject player;
    private bool isChasing = false; // ���� ������ ����
    private float stopChasingTime; // ���� ���� �ð�

    private Coroutine moveCoroutine;

  
    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        startPosition = transform.position; // ���� ��ġ ����
        player = GameObject.FindGameObjectWithTag("Player"); // �÷��̾� ã��
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRange)
        {
            

            if (enemyManager != null)
            {
                if (moveCoroutine == null) // �ߺ� ���� ����
                {
                    moveCoroutine = StartCoroutine(enemyManager.MoveCoroutine());
                    moveCoroutine = StartCoroutine(enemyManager.WalkCoroutine());
                }

                stopChasingTime = Time.time + returnDelay;
                isChasing = true;
            }
            else
            {
                Debug.LogError("EnemyManager�� ã�� �� �����ϴ�!");
            }
            
        }

        if (isChasing)
        {
            if (Time.time > stopChasingTime)
            {
                isChasing = false;
                startPosition = transform.position; // ���� ��ġ�� ���ο� ���� ��ġ�� ����
            }
        }
        else
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            Wander();
        }
      
    }


    void Wander()
    {
        float offsetX = Mathf.Sin(Time.time * speed) * range; // X������ �պ� �̵�
        transform.position = new Vector3(startPosition.x + offsetX, startPosition.y, startPosition.z);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (enemyManager != null)
        {
            enemyManager.SlowDown(); // ���� �ӵ� ����
        }
        else
        {
            Debug.LogError("EnemyManager�� ã�� �� �����ϴ�!");
        }

        if (Health <= 0)
        {
            Debug.Log($" {gameObject.name} óġ��!");
            Destroy(gameObject); // ���� ������ ����
        }
    }

    // ������: ���� �Ÿ��� ���� �Ÿ��� ������ Scene �信�� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        // �⺻ �̵� ������ ǥ���ϴ� ��Ȳ�� ��
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,range);

        
    }

}
