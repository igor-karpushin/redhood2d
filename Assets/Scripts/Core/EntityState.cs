namespace Adventure.Core
{
    public enum EntityState
    {
        None = 2 << 0,
        KinematicGrounded = 2 << 1,
        VelocityNegative = 2 << 2,
        VelocityPositive = 2 << 3,
        MovementPositive = 2 << 4,
        MovementTurn = 2 << 5,
        AttackLight = 2 << 6
    }
}