﻿using PainKiller.ThirdEyeAgentCommands.Enums;

namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class AnalyzeCache
{
    public List<ThirdPartyComponent> Components { get; set; }
    public CvssSeverity Severity { get; set; }
}