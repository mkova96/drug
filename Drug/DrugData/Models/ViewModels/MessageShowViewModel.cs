using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DrugData.Models.ViewModels
{
    public class MessageShowViewModel
    {
        public IEnumerable<Message> Messages { get; set; }
        public int CurrentMessageId { get; set; }
        public User Receiver { get; set; }

        [Required(ErrorMessage = "Morate unijeti tekst poruke")]
        public string About { get; set; }
        //public User UserRecieve { get; set; }

    }
}
