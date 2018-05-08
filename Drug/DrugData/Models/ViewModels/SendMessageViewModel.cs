using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class SendMessageViewModel
    {
        [Required(ErrorMessage = "Morate unijeti primatelja poruke")]
        public User User { get; set; }
        [Required(ErrorMessage = "Morate unijeti tekst poruke")]
        public string About { get; set; }
        //public User UserRecieve { get; set; }
    }
}
