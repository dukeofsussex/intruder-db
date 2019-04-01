// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    public interface IStats
    {
        string Identifier { get; set; }

        dynamic Item { get; set; }
    }
}
