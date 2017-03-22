using AInBox.Astove.Core.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AInBox.Messaging.Data
{
    [Table("enterprise")]
    public class Enterprise : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required]
        [MaxLength(40)]
        [Index()]
        public string Name { get; set; }

        [Index("idx_username", IsUnique = true)]
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MaxLength(60)]
        public string Password { get; set; }

        [MaxLength(40)]
        public string FromName { get; set; }
        [MaxLength(40)]
        public string FromEmail { get; set; }
        [MaxLength(40)]
        public string Host { get; set; }
        public int? Port { get; set; }
        [MaxLength(20)]
        public string ServerUsername { get; set; }
        [MaxLength(60)]
        public string ServerPassword { get; set; }

        [NotMapped]
        public bool SSL { get { return Port.GetValueOrDefault(25) == 587; } }
        [NotMapped]
        public bool IsHostParameterized { get { return !(string.IsNullOrEmpty(FromEmail) || string.IsNullOrEmpty(Host) || !Port.HasValue); } }
        [NotMapped]
        public bool IsUsernameParameterized { get { return !(string.IsNullOrEmpty(ServerUsername) || string.IsNullOrEmpty(ServerPassword)); } }
    }
}
