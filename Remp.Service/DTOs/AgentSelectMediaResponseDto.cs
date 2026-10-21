using System;

namespace Remp.Service.DTOs;

public class AgentSelectMediaResponseDto
{
    public int Id {get;set;}
    public List<int> MediaAssetsId {get;set;} = new List<int>();
}
