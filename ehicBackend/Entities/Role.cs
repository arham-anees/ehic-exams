using EhicBackend.Entities.Common;

namespace  EhicBackend.Entities
{
    public class Role: BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}