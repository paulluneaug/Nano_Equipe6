using UnityEngine;

public class ScrollingEnemy : MovingEnemy
{
    [SerializeField] private Vector2 m_scrollingDirection = Vector2.down;

    protected override void Move(float deltaTime)
    {
        Vector3 positionDelta = deltaTime * m_speedFactor * m_scrollingDirection.XY0();

        if (m_useGlobalScrollFactor)
        {
            positionDelta *= m_globalVariablesScriptable.ScrollSpeed;
        }

        transform.position += positionDelta;
    }
}
