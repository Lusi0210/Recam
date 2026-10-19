using System;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface ICaseContactRepository
{
    void Add(CaseContact caseContact);
}
