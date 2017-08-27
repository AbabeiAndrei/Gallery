using ServiceStack.DataAnnotations;

namespace Gallery.DataLayer.Entities.Base
{
    [EnumAsInt]
    public enum RowState : short
    {
        Created = 0,
        Deleted = 1
    }

    [EnumAsInt]
    public enum Operation : short
    {
        Read,
        Update,
        Delete
    }

    public interface IEntity
    {
        RowState RowState { get; set; }
        bool HasAccess(IIdentity identity, Operation operation, object data = null);
    }
}
