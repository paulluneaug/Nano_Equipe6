public class NoShootPattern : ShootPattern
{
    public override bool UpdatePattern(float tdeltaTime)
    {
        // Does nothing
        return false;
    }
}
