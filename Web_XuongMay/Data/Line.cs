﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("Line")]
    public class Line
    {
        [Key]
        public Guid LineId { get; set; }
        [Required]
        [MaxLength(50)]
        public string NameLine { get; set; }

        public int Id { get; set; }
        [ForeignKey("Id")]
        public User User { get; set; }
    }
}