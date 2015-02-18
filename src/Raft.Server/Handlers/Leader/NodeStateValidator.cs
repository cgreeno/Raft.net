using Raft.Core;
using Raft.Server.Events;
using Raft.Server.InternalCommands;

namespace Raft.Server.Handlers.Leader
{
    /// <summary>
    /// 1 of 5 EventHandlers for scheduled state machine commands.
    /// Order of execution:
    ///     NodeStateValidator*
    ///     LogEncoder
    ///     LogWriter
    ///     LogReplicator
    ///     CommandFinalizer
    /// </summary>
    internal class NodeStateValidator : LeaderEventHandler
    {
        private readonly IRaftNode _raftNode;

        public NodeStateValidator(IRaftNode raftNode)
        {
            _raftNode = raftNode;
        }

        public override void Handle(CommandScheduled @event)
        {
            var internalCommand = @event.Command as IRaftInternalCommand;

            if (internalCommand != null)
                internalCommand.NodeAction(_raftNode);
            else
                _raftNode.ScheduleCommandExecution();
        }
    }
}