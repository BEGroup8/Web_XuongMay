using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_XuongMay.Data
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public Guid MaHH { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenMH { get; set; }

        public string MoTa { get; set; }


    }
}
