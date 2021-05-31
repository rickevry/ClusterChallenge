// -----------------------------------------------------------------------
// <copyright file="DeadLetter.cs" company="Asynkron AB">
//      Copyright (C) 2015-2020 Asynkron AB All rights reserved
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace

// ReSharper disable once CheckNamespace

using System;
using JetBrains.Annotations;

namespace Proto
{
    [PublicAPI]
    public class DeadLetterEvent
    {
        public DeadLetterEvent(PID pid, object message, PID? sender, string? msg) : this(pid, message, sender, MessageHeader.Empty, msg)
        {
        }

        public DeadLetterEvent(PID pid, object message, PID? sender, MessageHeader? header, string? msg)
        {
            Pid = pid;
            Reason = msg;
            Message = message;
            Sender = sender;
            Header = header ?? MessageHeader.Empty;
        }

        public PID Pid { get; }
        public String? Reason { get; }
        public object Message { get; }
        public PID? Sender { get; }
        public MessageHeader Header { get; }
    }

    public class DeadLetterProcess : Process
    {
        public DeadLetterProcess(ActorSystem system) : base(system)
        {
        }

        protected internal override void SendUserMessage(PID pid, object message)
        {
            var (msg, sender, header) = MessageEnvelope.Unwrap(message);
            System.EventStream.Publish(new DeadLetterEvent(pid, msg, sender, header, "SendUserMessage"));
            if (sender is null) return;

            System.Root.Send(sender, msg is PoisonPill
                ? new Terminated {Who = pid, Why = TerminatedReason.NotFound}
                : new DeadLetterResponse {Target = pid}
            );
        }

        protected internal override void SendSystemMessage(PID pid, object message)
            => System.EventStream.Publish(new DeadLetterEvent(pid, message, null, null));
    }

    public class DeadLetterException : Exception
    {
        public DeadLetterException(PID pid) : base($"{pid} no longer exists")
        {
        }
    }
}