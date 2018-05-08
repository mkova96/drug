using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class MessageViewModel
    {
            [Required(ErrorMessage = "Morate unijeti primatelja poruke")]
            public string ReceiverId { get; set; }
            [Required(ErrorMessage = "Morate unijeti tekst poruke")]
            public string About { get; set; }
            //public User UserRecieve { get; set; }
        

    }
}
