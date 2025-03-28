﻿using PainKiller.ThirdEyeAgentCommands.Contracts;
using PainKiller.ThirdEyeAgentCommands.DomainObjects;

namespace PainKiller.ThirdEyeAgentCommands.Data;

public class SoftwareObjects : IDataObjects<Software>
{
    public DateTime LastUpdated { get; set; }
    public List<Software> Items { get; set; } = [];
}