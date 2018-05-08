using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;



namespace LijekData.Models
{
    public class User : IdentityUser
    {

        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime UserDate { get; set; }

        public bool IsDoctor {get;set;}
        public bool IsAdmin { get; set; }


        [NotMapped]
        public virtual string FullName => $"{Name} {Surname}";


        public virtual ICollection<Comment> Comments { get; set; }


        [Required]
        public String Address { get; set; }

        public virtual City City { get; set; }
       // public virtual ICollection<Order> Orders { get; set; }

        //public int CartId { get; set; }
        // public virtual Cart Cart { get; set; }

    }
}
