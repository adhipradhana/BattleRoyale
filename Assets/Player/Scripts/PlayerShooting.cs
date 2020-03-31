using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    private const int BulletPackAddition = 5;
    private const int MinBulletCount = 0;

    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public Player sourcePlayer;

    private int bulletCount = 0;
    public int BulletCount
    {
        get { return bulletCount; }
        set { bulletCount = value; }
    }

    public void Shoot()
    {
        if (BulletCount > MinBulletCount)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

            BulletCount--;

            if (BulletCount < MinBulletCount)
            {
                BulletCount = MinBulletCount;
            }

            bullet.GetComponent<BulletBehaviour>().sourcePlayer = sourcePlayer;
        }
    }

    public void GetBulletPack()
    {
        BulletCount += BulletPackAddition;
    }
}
