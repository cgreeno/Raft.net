﻿using Raft.Infrastructure.Journaler;

namespace Raft.Configuration
{
    public interface IRaftConfiguration {
        JournalConfiguration JournalConfiguration { get; set; }
    }
}