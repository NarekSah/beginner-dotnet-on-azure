using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunsonPickles.Events
{
    public interface IEventGridPublisher
    {
        public Task PublishEventAsync<T>(T eventData) where T : class;
    }
}
