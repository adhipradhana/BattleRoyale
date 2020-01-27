using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    private const string BulletPackTag = "Bullet Pack";
    private const int BulletPackAddition = 5;
    private const int MinBulletCount = 0;

    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 20f;

    private int bulletCount = 10000;
    public int BulletCount
    {
        get { return bulletCount; }
        set { bulletCount = value; }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
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
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(BulletPackTag))
        {
            BulletCount += BulletPackAddition;
            Destroy(collision.gameObject);
        }
    }
}
