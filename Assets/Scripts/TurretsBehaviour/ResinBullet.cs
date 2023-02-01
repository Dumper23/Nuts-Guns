using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResinBullet : MonoBehaviour
{

    [SerializeField]
    private float slowness, duration;

    private float startDuration;

    private int damage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= startDuration + duration)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void SetDamage(int bulletDamage)
    {
        damage = bulletDamage;
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    private void OnEnable()
    {
        startDuration = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            //collision.gameObject.GetComponent<EnemyScript>().Slow() i dmg;
        }
    }
}
