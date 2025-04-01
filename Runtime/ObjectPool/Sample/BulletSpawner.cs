using System.Collections;
using UnityEngine;

namespace HarryUtils.ObjectPool.Sample {
    public class BulletSpawner : MonoBehaviour, Bullet.IProtocol {
        // ==================== VARIABLES ===================
        #region Private Variables
        [Header("Settings")]
        [SerializeField] private float bulletSpeed = 5f;
        [SerializeField] private float bulletLifetime = 2f;
        [SerializeField] private float fireRate = 1f;
        [Space]
        [SerializeField] private int maxBullets = 5;

        [Header("References")]
        [SerializeField] private Bullet bulletPrefab = null;

        private LimitedObjectPool<Bullet> bulletsPool;
        #endregion
        
        // ==================== START ====================
        private void Start() {
            bulletsPool = new(bulletPrefab, maxBullets / 2, maxBullets);
            StartCoroutine(StartShoot());
        }
        
        // ==================== METHODS ====================
        private IEnumerator StartShoot() {
            while (true) {
                if (fireRate < 0.1f) fireRate = 0.1f;
                yield return new WaitForSeconds(1f / fireRate);
                
                CreateBullet();
            }
        }

        private void CreateBullet() {
            bulletsPool.GetObject()
                .SetDelegate(this)
                .Shoot(transform, bulletSpeed, bulletLifetime);
        }

        // ==================== INTERFACES ====================
        void Bullet.IProtocol.OnStop(Bullet _bullet) {
            bulletsPool.ReleaseObject(_bullet);
        }
    }
}