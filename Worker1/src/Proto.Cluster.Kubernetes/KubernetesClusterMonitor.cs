// -----------------------------------------------------------------------
// <copyright file="KubernetesClusterMonitor.cs" company="Asynkron AB">
//      Copyright (C) 2015-2020 Asynkron AB All rights reserved
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Proto.Cluster.Events;
using static Proto.Cluster.Kubernetes.Messages;
using static Proto.Cluster.Kubernetes.ProtoLabels;

namespace Proto.Cluster.Kubernetes
{
    class KubernetesClusterMonitor : IActor
    {
        private static readonly ILogger Logger = Log.CreateLogger<KubernetesClusterMonitor>();
        private readonly Cluster _cluster;

        private readonly Dictionary<string, V1Pod> _clusterPods = new();
        private readonly IKubernetes _kubernetes;

        private string _address;

        private string _clusterName;
        private string _memberId;
        private string _podName;
        private bool _stopping;
        private Watcher<V1Pod> _watcher;
        private Task<HttpOperationResponse<V1PodList>> _watcherTask;
        private bool _watching;

        public KubernetesClusterMonitor(Cluster cluster, IKubernetes kubernetes)
        {
            _cluster = cluster;
            _kubernetes = kubernetes;
        }

        public Task ReceiveAsync(IContext context) => context.Message switch
        {
            RegisterMember cmd => Register(cmd),
            StartWatchingCluster cmd => StartWatchingCluster(cmd.ClusterName, context),
            DeregisterMember _ => StopWatchingCluster(),
            Stopping _ => StopWatchingCluster(),
            _ => Task.CompletedTask
        };

        private Task Register(RegisterMember cmd)
        {
            try
            {
                _clusterName = cmd.ClusterName;
                _address = cmd.Address;
                _podName = KubernetesExtensions.GetPodName();
                _memberId = cmd.MemberId;
            }
            catch (Exception e)
            {
                Logger.LogError("Exception in Register", e);
                Logger.LogError("Exception in Register" + e.ToString());
            }
            return Task.CompletedTask;
        }

        private Task StopWatchingCluster()
        {
            try
            {
                // ReSharper disable once InvertIf
                if (_watching)
                {
                    Logger.LogInformation("[Cluster] Stopping monitoring for {PodName} with ip {PodIp}", _podName, _address
                    );

                    _stopping = true;
                    _watcher.Dispose();
                    _watcherTask.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Exception in StopWatchingCluster", e);
                Logger.LogError("Exception in StopWatchingCluster" + e.ToString());
            }

            return Task.CompletedTask;
        }

        private Task StartWatchingCluster(string clusterName, ISenderContext context)
        {
            try
            {
                var selector = $"{LabelCluster}={clusterName}";
                Logger.LogDebug("[Cluster] Starting to watch pods with {Selector}", selector);

                _watcherTask = _kubernetes.ListNamespacedPodWithHttpMessagesAsync(
                    KubernetesExtensions.GetKubeNamespace(),
                    labelSelector: selector,
                    watch: true
                );
                _watcher = _watcherTask.Watch<V1Pod, V1PodList>(Watch, Error, Closed);
                _watching = true;
            }
            catch (Exception e)
            {
                Logger.LogError("Exception in StartWatchingCluster", e);
                Logger.LogError("Exception in StartWatchingCluster" + e.ToString());
            }

            void Error(Exception ex)
            {
                try
                {
                    // If we are already in stopping state, just ignore it
                    if (_stopping) return;

                    // We log it and attempt to watch again, overcome transient issues
                    Logger.LogInformation("[Cluster] Unable to watch the cluster status: {Error}", ex.Message);
                    Restart();
                }
                catch (Exception e)
                {
                    Logger.LogError("Exception in Error", e);
                    Logger.LogError("Exception in Error" + e.ToString());
                }
            }

            // The watcher closes from time to time and needs to be restarted
            void Closed()
            {
                try
                {
                    // If we are already in stopping state, just ignore it
                    if (_stopping) return;

                    Logger.LogDebug("[Cluster] Watcher has closed, restarting");
                    Restart();
                }
                catch (Exception e)
                {
                    Logger.LogError("Exception in Closed", e);
                    Logger.LogError("Exception in Closed" + e.ToString());
                }
            }

            void Restart()
            {
                try
                {
                    _watching = false;
                    _watcher?.Dispose();
                    _watcherTask?.Dispose();
                    context.Send(context.Self!, new StartWatchingCluster(_clusterName));
                }
                catch (Exception e)
                {
                    Logger.LogError("Exception in Restart", e);
                    Logger.LogError("Exception in Restart" + e.ToString());
                }
            }

            return Task.CompletedTask;
        }

        private void Watch(WatchEventType eventType, V1Pod eventPod)
        {
            try
            {
                var podLabels = eventPod.Metadata.Labels;

                if (!podLabels.TryGetValue(LabelCluster, out var podClusterName))
                {
                    Logger.LogInformation("[Cluster] The pod {PodName} is not a Proto.Cluster node", eventPod.Metadata.Name);
                    return;
                }

                if (_clusterName != podClusterName)
                {
                    Logger.LogInformation("[Cluster] The pod {PodName} is from another cluster {Cluster}",
                        eventPod.Metadata.Name, _clusterName
                    );
                    return;
                }

                Logger.LogInformation("[Rickard] Watch Cluster Topology:" + eventType);
                Logger.LogInformation("[Rickard] eventPod:" + eventPod.Uid());

                foreach (var key in _clusterPods.Keys)
                {
                    var item = _clusterPods[key];
                    Logger.LogInformation("[Rickard] pods before:" + item.Uid());
                }

                // Update the list of known pods
                if (eventType == WatchEventType.Deleted)
                    _clusterPods.Remove(eventPod.Uid());
                else
                    _clusterPods[eventPod.Uid()] = eventPod;

                foreach (V1Pod item in _clusterPods.Values)
                {
                    Logger.LogInformation("[Rickard] item:" + item.ToString());
                    
                    foreach (var status in item.Status.ContainerStatuses)
                    {
                        if (status != null && status.State != null && status.State.Terminated != null)
                        {
                            Logger.LogInformation("[Rickard] Terminated:" + status.State.Terminated.StartedAt);
                            Logger.LogInformation("[Rickard] Terminated:" + status.State.Terminated.ExitCode);
                            Logger.LogInformation("[Rickard] Terminated:" + status.State.Terminated.FinishedAt);
                            Logger.LogInformation("[Rickard] Terminated:" + status.State.Terminated.Signal);
                            Logger.LogInformation("[Rickard] Terminated:" + status.State.Terminated.Reason);
                            Logger.LogInformation("[Rickard] Terminated:" + status.State.Terminated.Message);
                        }
                        if (status != null && status.State != null && status.State.Running != null)
                        {
                            Logger.LogInformation("[Rickard] Running:" + status.State.Running.StartedAt);
                        }
                        if (status != null && status.State != null && status.State.Waiting != null)
                        {
                            Logger.LogInformation("[Rickard] Waiting:" + status.State.Waiting.Reason);
                            Logger.LogInformation("[Rickard] Waiting:" + status.State.Waiting.Message);
                        }
                        if (status != null)
                        {
                            Logger.LogInformation("[Rickard] Status:" + status.Ready);
                            Logger.LogInformation("[Rickard] Status:" + status.ToString());
                        }
                    }
                }

                var memberStatuses = _clusterPods.Values
                    .Select(x => x.GetMemberStatus())
                    .Where(x => x.IsCandidate)
                    .Select(x => x.Status)
                    .ToList();

                foreach (var member in memberStatuses)
                {
                    Logger.LogInformation("[Rickard] members:" + member.Id + "" + member.Host + "" + member.Kinds);
                }

                _cluster.MemberList.UpdateClusterTopology(memberStatuses, 0ul);
                var topology = new ClusterTopologyEvent(memberStatuses);
                _cluster.System.EventStream.Publish(topology);
            }
            catch (Exception e)
            {
                Logger.LogError("Exception in Watch", e);
                Logger.LogError("Exception in Watch" + e.ToString());
            }
        }
    }
}