using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineconeBullet : MonoBehaviour
{
    [Header("Bullet Stats")]

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float radius, bulletDuration;

    private Transform target;

    private int damage;

    private Vector2 lastPos, lastPos2;

    private bool lostTarget;

    private bool cluster;

    private float stunness;

    private bool isDestroying = false;


    [Header("Bullet Settings")]

    [SerializeField]
    private GameObject sprite;
    [SerializeField]
    private int numClusters;
    [SerializeField]
    private float minT, maxT, clusterSpeed, rotationModifier;
    [SerializeField]
    private Animator explosion;
    [SerializeField]
    private GameObject eSoundB;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isDestroying) {
            if (target != null && target.gameObject.activeInHierarchy && !lostTarget)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                lastPos = target.position;
                lastPos2 = transform.position;
            }
            else
            {
                transform.Translate((lastPos - lastPos2).normalized * moveSpeed * Time.deltaTime);
                //transform.position = Vector2.MoveTowards(transform.position, lastPos, moveSpeed * Time.deltaTime);
                lostTarget = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDestroying) {
            if (target != null && target.gameObject.activeInHierarchy && !lostTarget)
            {
                Vector3 vectorTarget = target.transform.position - transform.position;
                float angle = Mathf.Atan2(vectorTarget.y, vectorTarget.x) * Mathf.Rad2Deg - rotationModifier;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                sprite.transform.rotation = Quaternion.Slerp(sprite.transform.rotation, q, Time.deltaTime * 1000);
            }
            else
            {
                /*Vector3 vectorTarget = (lastPos - (Vector2)transform.position);
                float angle = Mathf.Atan2(vectorTarget.y, vectorTarget.x) * Mathf.Rad2Deg - rotationModifier;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * moveSpeed);*/
            }
        }
    }

    private void OnEnable()
    {
        target = null;
        lostTarget = false;
        Invoke("Disappear", bulletDuration);
    }

    private void Destroy()
    {
        isDestroying = true;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D c in enemies)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (c != null && c.GetComponent<Enemy>() != null)
                {
                    c.GetComponent<Enemy>().takeDamage(damage);
                    c.GetComponent<Enemy>().stun(stunness);
                    explosion.gameObject.SetActive(true);
                    explosion.Play("Explosion");
                    Instantiate(eSoundB);
                    sprite.SetActive(false);
                }
            }
        }
        if (cluster)
        {
            for (int i = 0; i < numClusters; i++)
            {
                GameObject bul = BulletPool.bulletPoolInstance.GetCluster();
                bul.transform.position = transform.position;

                bul.GetComponent<ClusterBullet>().SetDamage(damage);
                bul.GetComponent<ClusterBullet>().SetRadius(radius);
                bul.GetComponent<ClusterBullet>().SetSpeed(clusterSpeed);
                bul.GetComponent<ClusterBullet>().SetTime(Random.Range(minT, maxT));
                bul.GetComponent<ClusterBullet>().SetDirection(new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000)).normalized);
                bul.SetActive(true);
            }
        }
        Invoke("Disable", 1.1f);
    }

    private void Disable()
    {
        isDestroying = false;
        sprite.SetActive(true);
        explosion.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Disappear()
    {
        gameObject.SetActive(false);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetDamage(int bulletDamage)
    {
        damage = bulletDamage;
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;
    }

    public void SetCluster(bool newCluster)
    {
        cluster = newCluster;
    }
    public void SetStunness(float newStun)
    {
        stunness = newStun;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (lostTarget)
            {
                //collision.gameObject.GetComponent<TMPEnemy>().Damage(damage);
                //collision.gameObject.GetComponent<EnemyScript>().Damage();
                Destroy();
            }
            else
            {
                if (collision.gameObject.transform == target)
                {
                    //collision.gameObject.GetComponent<TMPEnemy>().Damage(damage);
                    //collision.gameObject.GetComponent<EnemyScript>().Damage();
                    Destroy();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}