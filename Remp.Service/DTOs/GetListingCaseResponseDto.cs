using System;

namespace Remp.Service.DTOs;

public class GetListingCaseResponseDto
{
    public int Id {get;set;}
    public string Title {get;set;} = null!;
    public string Street {get;set;} = null!;
    public string City {get;set;} = null!;
    public string State {get;set;} = null!;

}
