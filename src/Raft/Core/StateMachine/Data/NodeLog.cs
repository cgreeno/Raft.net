﻿using System;

namespace Raft.Core.StateMachine.Data
{
    internal class NodeLog
    {
        private const int LogIncrementSize = 64;

        private RaftLogEntry[] _log = new RaftLogEntry[LogIncrementSize];

        public long? this[long commitIndex]
        {
            get
            {
                if (commitIndex == 0)
                    return 0;

                var logEntry = _log[commitIndex - 1];
                return logEntry.Set ? (long?)logEntry.Term : null;
            }
        }

        public void SetLogEntry(long commitIndex, long term)
        {
            if (commitIndex < 1)
                throw new IndexOutOfRangeException("Commit index for log must start from 1.");

            if (commitIndex > _log.Length)
            {
                var newLog = new RaftLogEntry[_log.Length + LogIncrementSize];
                _log.CopyTo(newLog, 0);
                _log = newLog;
            }

            _log[commitIndex - 1] = new RaftLogEntry(term);
        }

        public void TruncateLog(long truncateFromIndex)
        {
            for (var i = truncateFromIndex; i < _log.Length; i++)
                _log[i] = default(RaftLogEntry);
        }

        private struct RaftLogEntry
        {
            public RaftLogEntry(long term)
            {
                Term = term;
                Set = true;
            }

            public readonly long Term;

            public readonly bool Set;
        }
    }
}
