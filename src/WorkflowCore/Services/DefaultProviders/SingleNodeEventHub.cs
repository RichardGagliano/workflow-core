using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Services
{
    public class SingleNodeEventHub : ILifeCycleEventHub
    {
        private readonly ILogger<SingleNodeEventHub> _logger;
        private ICollection<Action<LifeCycleEvent>> _subscribers = new HashSet<Action<LifeCycleEvent>>();

        public SingleNodeEventHub(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<SingleNodeEventHub>();
        }

        public Task PublishNotification(LifeCycleEvent evt)
        {
            foreach (var subscriber in _subscribers)
            {
                try
                {
                    subscriber(evt);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error on event subscriber");
                }
            }

            return Task.CompletedTask;
        }

        public void Subscribe(Action<LifeCycleEvent> action)
        {
            _subscribers.Add(action);
        }

        public Task Start()
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            _subscribers.Clear();
            return Task.CompletedTask;
        }
    }
}
