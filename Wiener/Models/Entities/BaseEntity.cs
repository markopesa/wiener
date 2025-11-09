namespace Wiener.Models.Entities
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
