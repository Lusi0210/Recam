using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Remp.Service.DTOs;

public class AddAgentToPhotographyCompanyDto
{
    public string AgentId { get; set; } = null!;
    public string PhotographyCompanyId { get; set; } = null!;
}
