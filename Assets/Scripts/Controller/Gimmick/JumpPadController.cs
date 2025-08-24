using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class JumpPadController : BaseGimmickController
{
    float Speed = 0.15f; // 애니메이션 속도 조절을 위한 변수
    [Range(1f, 12f)]
    [SerializeField]
    float JumpForce = 10f; // 점프 패드가 적용할 점프 힘
    Vector3 _deafultScale;
    Vector3 _scaleStep1;
    Vector3 _scaleStep2;
    Vector3 _scaleStep3;
    Vector3 _scaleStep4;
    Vector3 _scaleStep5;
    Vector3 _scaleStep6;
    List<Collider> _objects = new List<Collider>();
    List<Vector3> Steps = new List<Vector3>();
    Collider _collider;
    [SerializeField]
    bool AutoJumping = false;
    enum JumpPadState
    {
        Default,
        Step1,
        Step2,
        Step3,
        Step4,
        Step5,
        Step6,
    }
    JumpPadState _state = JumpPadState.Default;
    Coroutine _coJump = null;
    public override void Enter()
    {
        if (_coJump == null)
            _coJump = StartCoroutine(PlayJumpPadAnim());
    }
    public override void Exit()
    {
    }
    protected override void Init()
    {
        // defaultScale -> step1 -> step2 -> step3 -> step4 -> defaultScale 순으로 점프 애니메이션 적용
        SetSteps();
        _collider = GetComponent<Collider>();
    }
    void SetSteps()
    {
        // 일단 상수로 처리
        _deafultScale = transform.localScale;
        _scaleStep1 = _deafultScale;
        _scaleStep1.y *= .6f;
        _scaleStep2 = _deafultScale;
        _scaleStep2.y *= 2f;
        _scaleStep3 = _deafultScale;
        _scaleStep3.y *= .8f;
        _scaleStep4 = _deafultScale;
        _scaleStep4.y *= 1.4f;
        _scaleStep5 = _deafultScale;
        _scaleStep5.y *= .9f;
        _scaleStep6 = _deafultScale;
        _scaleStep6.y *= 1.1f;

        _scaleStep1.y -= (_scaleStep1.y * (JumpForce * .05f));
        _scaleStep2.y += (_scaleStep2.y * (JumpForce * .05f));
        _scaleStep3.y -= (_scaleStep3.y * (JumpForce * .05f));
        _scaleStep4.y += (_scaleStep4.y * (JumpForce * .05f));
        _scaleStep5.y -= (_scaleStep5.y * (JumpForce * .05f));
        _scaleStep6.y += (_scaleStep6.y * (JumpForce * .05f));

        Steps = new List<Vector3>
        {
            _deafultScale,
            _scaleStep1,
            _scaleStep2,
            _scaleStep3,
            _scaleStep4,
            _scaleStep5,
            _scaleStep6,
            _deafultScale,
        };
    }
    private void FixedUpdate()
    {
        // 디버깅용
        if (Input.GetKey(KeyCode.Q) && _coJump == null)
        {
            _coJump = StartCoroutine(PlayJumpPadAnim());
        }
    }
    IEnumerator PlayJumpPadAnim()
    {
        AudioManager.Instance.PlaySFX("SFX_JumpPad");

        for (int i = 0; i < Steps.Count - 1; i++)
        {
            _state = (JumpPadState)(i % 7);
            float time = 0f;
            while (time < Speed)
            {
                float t = time / Speed;
                t = Mathf.SmoothStep(0, 1, t);
                transform.localScale = Vector3.Lerp(Steps[i], Steps[i + 1], t);
                time += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        transform.localScale = _deafultScale;
        _objects.Clear();
        _coJump = null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (AutoJumping && _coJump == null)
            _coJump = StartCoroutine(PlayJumpPadAnim());
        if ((int)_state % 2 == 0)
            return;
        if (_objects.Contains(collision.collider))
            return;
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null)
            return;
        // 한번 충돌했던 오브젝트는 다시 점프하지 않도록 처리
        _objects.Add(collision.collider);

        var targetPos = collision.transform.position;
        var direction = CalculateDirection(_collider, targetPos);
        var distance = Vector3.Distance(_collider.bounds.center, targetPos);
        rb.AddForce(JumpForce * Vector3.up, ForceMode.Impulse);
        rb.AddForce(direction * distance, ForceMode.Impulse);
    }
    private void OnCollisionStay(Collision collision)
    {
        // 점프대 애니메이션이 빨라 물체를 통과한 경우 위치 조정
        if (collision.collider.bounds.min.y < _collider.bounds.max.y)
        {
            var rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb == null || rb.velocity.y < 0)
                return;

            var position = collision.gameObject.transform.position;
            var max = _collider.bounds.max;
            position.y = max.y;
            var targetBounds = collision.collider.bounds;
            var offset = targetBounds.center.y - targetBounds.min.y;
            position.y += offset;
            collision.gameObject.transform.position = position;
        }
    }

    // 중심점과 targetPos점의 방향 벡터를 계산
    Vector3 CalculateDirection(Collider collider, Vector3 targetPos)
    {
        Vector3 center = collider.bounds.center;
        center.y = 0;
        targetPos.y = 0;
        //center.y = targetPos.y; // y축을 맞추기 위해 targetPos의 y값으로 설정
        Vector3 direction = targetPos - center;
        direction.Normalize();
        return direction;
    }
}
