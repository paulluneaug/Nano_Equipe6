using UnityEngine;

public class ShootPatternController : MonoBehaviour
{
    [SerializeField] private ShootPattern m_shootPattern;

    private void Update()
    {
        m_shootPattern.UpdatePattern(Time.deltaTime);
    }

}
