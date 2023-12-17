namespace Identity.Domain
{
    public class BaseEntity
    {
        public Guid Id { get; init; }//实体Id
        public BaseEntity() {
            Id = Guid.NewGuid();
        }
    }
}
