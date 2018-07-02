using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class EditCommentViewModel
    {
        public Medication Drug { get; set; }

        public int CommentId { get; set; }

        [Required(ErrorMessage = "Sadržaj komentara je obavezan.")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

    }
}
