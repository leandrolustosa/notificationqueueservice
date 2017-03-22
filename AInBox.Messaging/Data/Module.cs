using AInBox.Astove.Core.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AInBox.Messaging.Data
{
    [Table("module")]
    public class Module : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required]
        [Index("idx_enterpriseId_name", IsUnique = true, Order = 1)]
        public int EnterpriseId { get; set; }

        [ForeignKey("EnterpriseId")]
        public Enterprise Enterprise { get; set; }

        [Required]
        [MaxLength(30)]
        [Index("idx_enterpriseId_name", IsUnique = true, Order = 2)]
        public string Name { get; set; }

        [MaxLength(40)]
        public string FromName { get; set; }

        [MaxLength(40)]
        public string FromEmail { get; set; }

        [MaxLength(40)]
        public string RestrictIps { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
