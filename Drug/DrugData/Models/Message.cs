using System;
using System.Collections.Generic;
using System.Text;

namespace LijekData.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Body { get; set; }
        public DateTime MessageDate { get; set; }

        public User Sender { get; set; }
        public User Receiver { get; set; }

        public User GetOtherUser(string userId)
        {
            return Sender.Id != userId ? Sender : Receiver;
        }

    }
}
