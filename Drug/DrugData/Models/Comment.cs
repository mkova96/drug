using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LijekData.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        public Drug Drug { get; set; }

        public User User { get; set; }

        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}
