using System.ComponentModel.DataAnnotations;
using System.Web;

namespace E_Market.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Product
    {
        public int id { get; set; }
        public string image { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(".{3,50}")]
        public string name { get; set; }

        [Required(ErrorMessage = "*Required")]
        public double price { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string description { get; set; }

        [Required(ErrorMessage = "*Required")]
        public int category_id { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Category Category { get; set; }

        
            
    }
}
