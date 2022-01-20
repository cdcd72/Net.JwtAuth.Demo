namespace API.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public Role Role { get; set; } = null!;
    }
}
