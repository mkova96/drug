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
        public string Content { get; set; }
    }
}
