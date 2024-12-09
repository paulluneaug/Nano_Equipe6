using System.Collections;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void StartVFXLifeCycle(VFXControllerPool pool)
    {
        _ = StartCoroutine(ReleaseCoroutine(pool));
    }

    private IEnumerator ReleaseCoroutine(VFXControllerPool pool)
    {
        AnimatorClipInfo[] animClipInfo = m_animator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(animClipInfo[0].clip.length);

        pool.Release(this);
    }
}
