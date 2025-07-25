﻿using PainKiller.ThirdEyeClient.Contracts;

namespace PainKiller.ThirdEyeClient.Data;

public class TeamObjects : IDataObjects<Team>
{
    public DateTime LastUpdated { get; set; }
    public List<Team> Items { get; set; } = [];
}