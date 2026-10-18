using CPCompanion.Application.DTOs.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Interfaces
{
    public interface IStatsService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(Guid userId);
    }
}
