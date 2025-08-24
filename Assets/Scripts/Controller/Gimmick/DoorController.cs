using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CanPositionDoorController : PositioningObjectController
{
    [SerializeField]
    string[] IgnoreLayers;
    [SerializeField]
    int RequireCount = 1;

    int _currentCount = 0;
    protected override void Init()
    {
        base.Init();
        foreach (string layerName in IgnoreLayers)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer($"{layerName}"), LayerMask.NameToLayer("Door"), true);
    }
    public override void Enter()
    {
        _currentCount = math.clamp(0, RequireCount, _currentCount + 1);

        if (_currentCount == RequireCount)
        {
            if (_coPositioning != null)
                StopCoroutine(_coPositioning);
            _coPositioning = StartCoroutine(CoMoveAt(To));
            AudioManager.Instance.PlaySFX("SFX_DoorOpen");
        }
    }
    public override void Exit()
    {
        _currentCount = math.clamp(0, RequireCount, _currentCount - 1);

        if (_coPositioning != null)
            StopCoroutine(_coPositioning);
        _coPositioning = StartCoroutine(CoMoveAt(From));
    }
}
