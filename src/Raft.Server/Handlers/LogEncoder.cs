﻿using System.IO;
using ProtoBuf;
using Raft.Core;
using Raft.Server.Log;

namespace Raft.Server.Handlers
{
    /// <summary>
    /// 2 of 5 EventHandlers for scheduled state machine commands.
    /// Order of execution:
    ///     NodeStateValidator
    ///     LogEncoder*
    ///     LogReplicator
    ///     LogWriter
    ///     CommandFinalizer
    /// </summary>
    internal class LogEncoder : RaftEventHandler, ISkipInternalCommands
    {
        private readonly IRaftNode _raftNode;
        private readonly LogRegister _logRegister;

        public LogEncoder(IRaftNode raftNode, LogRegister logRegister)
        {
            _raftNode = raftNode;
            _logRegister = logRegister;
        }

        // TODO: Should add checksum for validation when sourcing from log... http://stackoverflow.com/questions/10335203/is-there-any-very-rapid-checksum-generation-algorithm
        public override void Handle(CommandScheduledEvent @event)
        {
            var logEntry = new LogEntry {
                Term = _raftNode.CurrentTerm,
                Index = _raftNode.CommitIndex + 1,
                CommandType = @event.Command.GetType().AssemblyQualifiedName,
                Command = @event.Command
            };

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, logEntry);
                _logRegister.AddEncodedLog(@event.Id, ms.ToArray());
            }
        }
    }
}
