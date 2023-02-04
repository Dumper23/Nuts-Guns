using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineconeTurret : TurretsFather
{
    [SerializeField]
    private bool cluster = false;
    private float radius = 5;
    private void Awake()
    {
        turretId = "PINECONE_LAUNCHER";
    }

    void Start()
    {
        //ammunituion = 5;
        //damage = 10;
        base.Start();

    }

    void Update()
    {
        base.Update();
    }

    protected override void Shoot()
    {
        base.Shoot();

        GameObject bul = BulletPool.bulletPoolInstance.GetPinecone();
        bul.transform.position = GetBulletSpawnPoint().position;

        bul.SetActive(true);
        bul.GetComponent<PineconeBullet>().SetTarget(base.currentTarget.transform);
        bul.GetComponent<PineconeBullet>().SetDamage(base.damage);
        bul.GetComponent<PineconeBullet>().SetRadius(radius);
        bul.GetComponent<PineconeBullet>().SetCluster(cluster);
        ammunituion--;
    }

    protected override void InintiateStatsAtCurrentUpgrades()
    {
        Dictionary<string, float> d = GameManager.Instance.getTurretInfo(turretId);

        foreach (KeyValuePair<string, float> stat in d)
        {
            switch (stat.Key)
            {
                case "capacity":
                    if ((int)stat.Value != 0)
                    {
                        maxAmmo = (int)stat.Value;
                    }
                    break;
                case "damage":
                    if ((int)stat.Value != 0)
                    {
                        damage = (int)stat.Value;
                    }
                    break;
                case "area":
                    if (stat.Value != 0)
                    {
                        radius = stat.Value;
                    }
                    break;
                case "cluster":
                    if (stat.Value == 0)
                    {
                        cluster = false;
                    }
                    if (stat.Value == 1)
                    {
                        cluster = true;
                    }
                    break;
            }
        }
    }
    public override void SetTraits(int newmaxAmmo, int newDamage, float newRadius, bool newCluster = false, float none = -1, float none1 = -1, float none2 = -1)
    {
        maxAmmo = newmaxAmmo;
        damage = newDamage;
        radius = newRadius;
        cluster = newCluster;
    }
    protected override void DetectObjective()
    {
        int closestPos = -1;
        float closestDistance = Mathf.Infinity;
        int posIterator = 0;

        foreach (GameObject o in enemyList)
        {
            if (o != null && o.activeInHierarchy)
            {
                if (IsInRange(o))
                {

                    if (closestDistance > Vector2.Distance(transform.position, o.transform.position))
                    {
                        closestDistance = Vector2.Distance(transform.position, o.transform.position);
                        closestPos = posIterator;
                    }
                }
            }
            posIterator++;
        }
        if (closestPos != -1)
        {
            //Attack
            currentTarget = enemyList[closestPos];
        }
        else
        {
            //No attack
            currentTarget = null;
        }
    }
}
