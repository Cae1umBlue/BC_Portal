using System.Collections;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class StageSound : MonoBehaviour
{
    private bool on = false;
    [SerializeField] private int targetLayer;

    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (!on && other.gameObject.layer == targetLayer)
        {
            on = true;
            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);

        if (animator != null)
        {
            animator.SetTrigger("On");
        }
        AudioManager.Instance.PlaySFX("SFX_Sys_On");
    }
}
