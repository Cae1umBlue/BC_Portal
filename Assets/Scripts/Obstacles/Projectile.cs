using UnityEngine;
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    private float speed = 23f; // 발사체 속도
    private float lifeTime = 2f; // 발사체 지속시간
    private float damage = 40f; // 발사체 데미지

    public GameObject hitEffectPrefab; // 충돌 시 이펙트
    private Vector3 direction; // 발사체 방향

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    public void Init(Vector3 dir) // 발사체 프리팹 초기화
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject _target = other.gameObject;
        if(LayerMask.LayerToName(_target.layer) == "Player")
        {
            PlayerManager.Instance.Player.condition.TakePhysicalDamage(damage);
        }
        AudioManager.Instance.PlaySFX("SFX_Laser_Hit");
        if (hitEffectPrefab != null)
        {
            // 프로젝타일, 이펙트 생성/파괴가 자주 일어날 것 오브젝트 폴링 고려
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

}
