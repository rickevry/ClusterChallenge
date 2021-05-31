using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;

namespace DAM2.Core.Shared.Generic
{

    public class GenericActor
    {
        private readonly ILogger _logger;

        protected PIDValues pidValues;
        protected Props _childFactory = null;
        private Dictionary<string, PID> _actors = null;
        public GenericActor(ILogger logger = default)
        {
            _logger = logger;
            _actors = new Dictionary<string, PID>();
        }

        public virtual Task ReceiveAsync(IContext context) =>
           context.Message switch
           {
               Proto.Cluster.ClusterInit cmd => ClusterInit(context, cmd),
               Stopping _ => Stopping(context),
               Stopped _ => Stopped(context),
               Restarting _ => Restarting(context),
               ReceiveTimeout _ => ReceiveTimeout(context),
               _ => UnknownCmd(context, context.Message)
           };


        private string CreateKey(string tenant, string name, string eid)
        {
            return $"@{tenant}:@{name}:@{eid}";
        }

        protected PID GetActor(IContext context, string tenant, string name, string eid, string objectId)
        {
            var key = CreateKey(tenant, name, eid);
            if (!_actors.ContainsKey(key))
            {
                var newChildActor = context.Spawn(_childFactory);
                _actors.Add(key, newChildActor);
                context.Send(newChildActor, new LoadStateCmd(tenant, name, eid, objectId));
            }
            return _actors[key];
        }

        protected PID GetQueryActor(IContext context, string tenant, string name, string queryStringBson)
        {
            if (queryStringBson == null)
            {
                throw new ArgumentNullException(nameof(queryStringBson));
            }
            var hash = queryStringBson.GetHashCode(StringComparison.Ordinal);
            var key = $"/query/{tenant}/{name}/{hash}";
            if (!_actors.ContainsKey(key))
            {
                var newChildActor = context.Spawn(_childFactory);
                _actors.Add(key, newChildActor);
            }
            return _actors[key];
        }

        protected Task Stopped(IContext context)
        {
            return Task.CompletedTask;
        }

        protected Task ClusterInit(IContext context, Proto.Cluster.ClusterInit cmd)
        {
            return Task.CompletedTask;
        }

        protected Task Stopping(IContext context)
        {
            return Task.CompletedTask;
        }
        protected Task Restarting(IContext context)
        {
            return Task.CompletedTask;
        }

        protected Task ReceiveTimeout(IContext context)
        {
            return Task.CompletedTask;
        }

        protected Task UnknownCmd(IContext context, object cmd)
        {
            return Task.CompletedTask;
        }

        protected virtual Task Started(IContext context)
        {
            this.pidValues = context.Self.ExtractIdValues();

            _logger.LogInformation($"GenericActor - Started");
            return Task.CompletedTask;
        }

        protected Task GarbageCollect(IContext context, GarbageCollectCmd cmd)
        {
            _logger.LogInformation("It is time to Garbage Collect");
            return Task.CompletedTask;
        }


        protected bool GetParams(IContext context, out string p1)
        {
            string[] cmdParts = context.Self.Id.Split("$");
            if (cmdParts.Length > 0)
            {
                string[] parts = cmdParts[0].Split('/');

                if (parts.Length > 1)
                {
                    p1 = parts[parts.Length-1];
                    return true;
                }
            }
            p1 = null;
            return false;
        }

        protected bool GetParams(IContext context, out string p1, out string p2)
        {
            string[] cmdParts = context.Self.Id.Split("$");
            if (cmdParts.Length > 0)
            {
                string[] parts = cmdParts[0].Split('/');

                if (parts.Length > 2)
                {
                    p1 = parts[parts.Length - 2];
                    p2 = parts[parts.Length - 1];
                    return true;
                }
            }
            p1 = null;
            p2 = null;
            return false;
        }

        //string tenant = parts[parts.Length - 2];
        //string eid = parts[parts.Length - 1];
    }
}
