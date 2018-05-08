using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class CommentViewModel
    {
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public Medication Drug { get; set; }

        public User User { get; set; }
    }
}
