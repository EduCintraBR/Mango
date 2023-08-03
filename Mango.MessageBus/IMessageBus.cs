using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public interface IAzureMessageBus
    {
        Task PublishMessage(object message, string topic_queue_name);
    }
}
