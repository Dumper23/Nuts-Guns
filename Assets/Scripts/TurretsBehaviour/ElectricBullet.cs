using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField]
    private float moveSpeed, bulletDuration, radiusDetection;
    //NEED ROTATION?
    private Transform target;

    private int damage;

    private bool lostTarget;

    private List<Transform> enemisDone;

    private float stunness;

    private int hitsRay, currentHits;

    [Header("Bullet Settings")]

    [SerializeField]
    private TrailRenderer tr;
    [SerializeField]
    private GameObject sprite;
    [SerializeField]
    private float rotationModifier;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!lostTarget) {
            if (target != null && target.gameObject.activeInHierarchy)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                lostTarget = true;
                SearchNewTarget();
            }
        }
    }
    private void FixedUpdate()
    {
        Vector3 vectorTarget = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(vectorTarget.y, vectorTarget.x) * Mathf.Rad2Deg - rotationModifier;
        sprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnEnable()
    {
        tr.Clear();
        target = null;
        lostTarget = false;
        enemisDone = new List<Transform>();
        currentHits = hitsRay;
        Invoke("Destroy", bulletDuration);
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        enemisDone.Add(target);
    }

    public void SetDamage(int bulletDamage)
    {
        damage = bulletDamage;
    }

    public void SetStun(float stun)
    {
        stunness = stun;
    }

    public void SetHitsRay(int newHits)
    {
        hitsRay = newHits;
    }

    private void SearchNewTarget()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, radiusDetection);
        List<Transform> enemies = new List<Transform>();
        foreach (Collider2D c in colls)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                enemies.Add(c.gameObject.transform);
            }
        }
        int maxLoops = 100;
        int loop = 0;
        if (enemies.Count > 0) {
            Transform tmp = null;
            bool correct = false;
            while (!correct && loop < maxLoops)
            {
                tmp = enemies[Random.Range(0, enemies.Count)];
                bool found = false;
                foreach (Transform enemy in enemisDone)
                {
                    if (enemy == tmp)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    correct = true;
                }
                loop++;
            }
            if (!correct)
            {
                Destroy();
            }
            else
            {
                target = tmp;
                enemisDone.Add(tmp);
                lostTarget = false;
            }
        }
        else
        {
            Destroy();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.gameObject.transform == target)
            {
                currentHits--;
                collision.gameObject.GetComponent<Enemy>().takeDamage(damage);
                //STUN
                lostTarget = true;
                SearchNewTarget();
                if (currentHits <= 0)
                {
                    Destroy();
                }
            }

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.gameObject.transform == target)
            {
                currentHits--;
                collision.gameObject.GetComponent<Enemy>().takeDamage(damage);
                //STUN
                Debug.Log("STAY");
                lostTarget = true;
                SearchNewTarget();
                if (currentHits <= 0)
                {
                    Destroy();
                }
            }
        }
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radiusDetection);
    }
}
