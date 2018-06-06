using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multiblog.Core.Model
{
    public class MessageItem
    {
        public string MessageId { get; set; }
        public string MessageText { get; set; }
        public DateTime CreatedOn { get; } = DateTime.UtcNow;
    }
}
