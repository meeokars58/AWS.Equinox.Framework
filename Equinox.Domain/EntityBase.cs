namespace Equinox.Domain;

public abstract class EntityBase : IEntity
{
    public virtual long Id { get; set; }
}