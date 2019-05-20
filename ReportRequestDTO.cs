using System;

public class ReportRequestDTO
{
    public bool Succeeded { get; set; }

    public IEnumerable<string> Messages { get; set; }
}
