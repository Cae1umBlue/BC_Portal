using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortalPlacement : MonoBehaviour
{
    [Header("포탈 프리팹")]
    [SerializeField] private Portal portalPrefabA;
    [SerializeField] private Portal portalPrefabB;

    [Header("Setup")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform mainCameraTransform;

    // 생성된 포탈 인스턴스를 저장할 변수
    private Portal portalInstanceA;
    private Portal portalInstanceB;

    private Animator portalanimatorA;
    private Animator portalanimatorB;


    private void Start()
    {
        // 게임 시작 시 포탈 프리팹으로부터 인스턴스를 생성해 둡니다.
        portalInstanceA = Instantiate(portalPrefabA);
        portalInstanceB = Instantiate(portalPrefabB);

        portalanimatorA = portalInstanceA.GetComponent<Animator>();
        portalanimatorB = portalInstanceB.GetComponent<Animator>();

        // 두 포탈이 서로를 알도록 연결해 줍니다.
        portalInstanceA.LinkedPortal(portalInstanceB);
        portalInstanceB.LinkedPortal(portalInstanceA);
    }

    public void OnFirePortal1(InputAction.CallbackContext context)
    {
        if (UIManager.Instance.isMenuOpen) return;
        if (context.phase == InputActionPhase.Started)
        {
            FirePortal(0, mainCameraTransform.transform.position, mainCameraTransform.transform.forward, 50.0f);
        }
    }

    public void OnFirePortal2(InputAction.CallbackContext context)
    {
        if (UIManager.Instance.isMenuOpen) return;
        if (context.phase == InputActionPhase.Started)
        {
            FirePortal(1, mainCameraTransform.transform.position, mainCameraTransform.transform.forward, 50.0f);
        }
    }

    // 포탈을 발사하는 함수
    // 델리게이트에 등록해도 괜찮을 것 같음
    // 설치할 포탈의 종류, 광선이 시작될 월드 좌표, 광선이 발사될 월드 방향, 광선의 최대 거리
    private void FirePortal(int portalId, Vector3 pos, Vector3 dir, float distance)
    {
        if (Physics.Raycast(pos, dir, out RaycastHit hit, distance, layerMask))
        {
            if (hit.collider.CompareTag("Portal")) // Ray를 쏜 곳이 포탈이라면 아무것도 하지 않는다.
            {
                return;
            }

            //portalId에 따라 배치할 포탈 인스턴스를 선택합니다.
            Portal portalToPlace;
            if (portalId == 0)
            {
                portalToPlace = portalInstanceA;
                portalanimatorA.SetTrigger("reset");
            }
            else
            {
                portalToPlace = portalInstanceB;
                portalanimatorB.SetTrigger("reset");
            }

            // 카메라 방향을 기준으로 포탈의 회전 값을 계산한다. (우선 현재 카메라의 회전 값을 가져온다.)
            var cameraRotation = mainCameraTransform.transform.rotation;
            // 카메라의 로컬 오른족 방향이 월드 공간에서 어느 방향인지 확인한다.
            var portalRight = cameraRotation * Vector3.right;

            // 생성될 포탈이 X 축에 더 가까운지 Z축에 더 가까운지 확인한다.
            if (Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                // X축에 더 가까울 때 x가 0보다 크거나 같으면 최종적으로 포탈의 오른쪽 방향을 결정한다.
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                // Z축에 더 가까울 때 z가 0보다 크거나 같으면 최종적으로 포탈의 앞쪽 뱡향을 결정한다.
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            // 광선이 부딪힌 벽의 바깥 면으로 포탈의 앞 면이 되도록 설정한다.
            var portalForward = -hit.normal;
            // 위에서 계산한 오른쪽, 앞쪽 방향에 모두 수직인 "위쪽" 방향을 설정한다. 
            var portalUp = -Vector3.Cross(portalRight, portalForward);
            // 그리고 위에서 계산한 앞쪽, 위쪽 방향을 사용해서 포탈의 회전값을 결정한다.
            var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

            // 계산된 위치와 회전 값으로 포탈 배치를 시도합니다.
            bool wasPlaced = portalToPlace.PlacePortal(hit.collider, hit.point, portalRotation);

            if (wasPlaced)
            {
                portalToPlace.transform.SetParent(hit.collider.transform, true); // true는 월드 위치를 유지하기 위함
                AudioManager.Instance.PlaySFX("SFX_Portal");
            }
        }
    }
}
