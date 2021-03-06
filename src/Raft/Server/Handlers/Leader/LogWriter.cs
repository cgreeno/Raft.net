﻿using System;
using Raft.Infrastructure.Journaler;
using Raft.Server.BufferEvents;

namespace Raft.Server.Handlers.Leader
{
    /// <summary>
    /// 2 of 4 EventHandlers for scheduled state machine commands.
    /// Order of execution:
    ///     LogEncoder
    ///     LogWriter*
    ///     LogReplicator
    ///     CommandFinalizer
    /// </summary>
    internal class LogWriter : LeaderEventHandler
    {
        private readonly IWriteDataBlocks _writeDataBlocks;

        public LogWriter(IWriteDataBlocks writeDataBlocks)
        {
            _writeDataBlocks = writeDataBlocks;
        }

        public override void Handle(CommandScheduled @event)
        {
            if (@event.LogEntry == null || @event.EncodedEntry == null)
                throw new InvalidOperationException("Must set EncodedEntry on event before executing this step.");

            _writeDataBlocks.WriteBlock(@event.EncodedEntry);
        }
    }
}
