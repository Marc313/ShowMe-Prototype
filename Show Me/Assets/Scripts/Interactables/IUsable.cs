public interface IUsable : IPickupable
{
    public float cooldown { get; }
    public void OnUse(Player _user);
}
