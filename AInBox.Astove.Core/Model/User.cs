using System;
using System.ComponentModel.DataAnnotations;

namespace AInBox.Astove.Core.Model
{
    public class User : BaseModel
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
