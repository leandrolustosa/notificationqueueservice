using AInBox.Astove.Core.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AInBox.Messaging.Data
{
    [Table("email")]
    public class Email : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public Module Module { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReferenceName { get; set; }

        [Required]
        public int ReferenceId { get; set; }

        [Column(TypeName = "text")]
        public string ReferenceParameters { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? NotificationDate { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }

        [MaxLength(10)]
        public string SubjectEncoding { get; set; }

        [MaxLength(10)]
        public string BodyEncoding { get; set; }

        [Required]
        public bool IsBodyHtml { get; set; }
        
        [Required]
        [Column(TypeName = "text")]
        public string To { get; set; }

        [Column(TypeName = "text")]
        public string Cc { get; set; }

        [Column(TypeName = "text")]
        public string Bco { get; set; }
    }
}
