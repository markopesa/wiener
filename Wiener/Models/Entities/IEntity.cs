namespace Wiener.Models.Entities
{
    public interface IEntity<TTypeOfKey>
    {
        public TTypeOfKey Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
