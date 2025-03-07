﻿namespace PainKiller.ThirdEyeAgentCommands.DomainObjects;

public class DevProject
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string Sdk { get; set; } = "";
    public string Version { get; set; } = "";
    public string Framework { get; set; } = "";
    public string Language { get; set; } = "";
    public List<ThirdPartyComponent> Components { get; set; } = [];

}