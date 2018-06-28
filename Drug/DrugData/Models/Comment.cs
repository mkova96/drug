using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        public Medication Drug { get; set; }

        public User User { get; set; }

        [Required(ErrorMessage = "Sadržaj komentara je obavezan.")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
