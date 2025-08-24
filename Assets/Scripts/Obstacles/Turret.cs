using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : ObstacleBase
{
    [Header("Turret Parts")]
    [SerializeField] private Transform turretBase; // 고정대
    [SerializeField] private Transform turretHead; // 본체

    // 파라미터를 SerializeField 로 만들어 외부에서 설정/테스트 가능 - 코드작업 리소스가 줄고 기획 및 테스터가 조정가능함 
    [Header("Turret Settings")]
    [SerializeField] private float rotationSpeed; // 터렛 회전 속도
    [SerializeField] private float idleRotationSpeed; // 터렛 대기 상태 회전 속도
    [SerializeField] private float detectionRange; // 터렛 감지 거리
    [SerializeField] private Transform target; // 타겟(플레이어)
    [SerializeField] private LayerMask targetLayerMask = 0; // Player 레이어

    [Header("Turret Fire Settings")]
    [SerializeField] private GameObject laserPrefab; // 레이저 투사체 프리팹
    [SerializeField] private Transform firePoint; // 레이저 발사 위치
    [SerializeField] private float fireCooldown = 1f; // 레이저 발사 주기

    private float lastFireTime = 0f;

    [SerializeField] private LaserPointer laserPointer; // 레이저 포인터

    [SerializeField] private bool showGizmo = true; // 기즈모 on/off

    public ParticleSystem MuzzleFlash;

    private string currentSound = "";

    private void Awake()
    {
        if (laserPointer == null)
            laserPointer = GetComponent<LaserPointer>();
    }

    public override void Activate() //터렛 On
    {
        base.Activate();
        // 문자열 사용 주의 nameof 고려
        InvokeRepeating("FindTarget", 0, 0.5f);
    }

    public override void Deactivate() // 터렛 Off
    {
        base.Deactivate();
        CancelInvoke("FindTarget");
        laserPointer.ClearTarget();
    }

    private void Start()
    {
        Activate();
    }

    private void Update()
    {
        RotateTurret();
    }

    // 가장 가까운 타겟을 탐색(추후 확장을 위해 단일이 아닌 배열로 설정)
    private void FindTarget() 
    {
        Collider[] _targetCollders = Physics.OverlapSphere(transform.position, detectionRange, targetLayerMask);  // 터렛 주변 콜라이더 탐색
        Transform _shortestTarget = null; // 가장 가까운 타겟

        // 타겟 콜라이더 탐색
        if (_targetCollders.Length > 0)
        {
            float _shortestTargetDistance = Mathf.Infinity;
            foreach(Collider _targetColider in _targetCollders) // 콜라이더를 찾을 때까지 탐색 반복
            {
                float _targetDistance = Vector3.SqrMagnitude(transform.position - _targetColider.transform.position); // 터렛과 타겟과의 거리
                if(_shortestTargetDistance > _targetDistance)
                {
                    _shortestTargetDistance = _targetDistance;
                    _shortestTarget = _targetColider.transform;
                }
            }
        }

        target = _shortestTarget;
    }

    private void RotateTurret()
    {
        Quaternion _turretMode = RotateMode();
        HandleRotateSound();
        RotateSpeed(_turretMode);
        HandleLaserPointer(_turretMode);
    }

    // 터렛 사운드
    // 대기 모드일 때는 Idle 사운드, 추적 모드일때는 Track 사운드
    private void HandleRotateSound()
    {
        string _newSound = target == null ? "SFX_Turret_IdleRotate" : "SFX_Turret_TrackRotate";

        if (_newSound != currentSound)
        {
            AudioManager.Instance.StopSFX(currentSound);

            if (_newSound == "SFX_Turret_IdleRotate")
                AudioManager.Instance.PlayLoopSFX(_newSound);
            else
                AudioManager.Instance.PlaySFX(_newSound);

            currentSound = _newSound;
        }
    }

    // 터렛 모드별 회전 상태
    // 대기 모드, 추적 모드
    private Quaternion RotateMode()
    {
        if (target == null)
        {
            laserPointer.ClearTarget();
            return turretHead.rotation * Quaternion.Euler(0, 1f, 0);
        }
        else
        {
            if(Camera.main != null)
            {
                Vector3 CameraPos = Camera.main.transform.position;
                CameraPos.y -= 1.0f;
                Vector3 _direction = CameraPos - turretHead.position;
                return Quaternion.LookRotation(_direction);
            }
            return turretHead.rotation;
        }
    }

    // 터렛 회전 속도
    // 터렛 모드에 따라 회전 속도 차이 
    private void RotateSpeed(Quaternion targetRotation)
    {
        float _currentSpeed = target == null ? idleRotationSpeed : rotationSpeed;
        turretHead.rotation = Quaternion.RotateTowards(turretHead.rotation, targetRotation, _currentSpeed * Time.deltaTime);
    }

    // 터렛 레이저 포인터 작동
    private void HandleLaserPointer(Quaternion targetRotation)
    {
        if (target == null) return;

        laserPointer.Activate();
        laserPointer.SetTarget(Camera.main.transform);
        Fire();
    }

    public void Fire()
    {
    // 발사 조건 확인
    if (target == null || Time.time < lastFireTime + fireCooldown)
        return;

        // 발사 방향 및 회전 계산
        Vector3 CameraPos = Camera.main.transform.position;
        CameraPos.y -= 1.0f;
        Vector3 _direction = (CameraPos - firePoint.position).normalized;
    Quaternion _rotation = Quaternion.LookRotation(_direction) * Quaternion.Euler(90f, 0f, 0f);

    // 사운드 및 이펙트 재생
    AudioManager.Instance.PlaySFX("SFX_Turret_Fire");
    MuzzleFlash?.Play();

    // 레이저 생성 및 초기화
    GameObject _laser = Instantiate(laserPrefab, firePoint.position, _rotation);
    Projectile _projectile = _laser.GetComponent<Projectile>();
    _projectile?.Init(_direction);

    // 마지막 발사 시간 갱신
    lastFireTime = Time.time;
  
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        Gizmos.color = new Color(0,1,0,0.3f);
        Gizmos.DrawSphere(transform.position, detectionRange);
    }

}
