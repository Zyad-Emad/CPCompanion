using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.Interfaces
{
    public interface ISyncService
    {
        Task SyncUserAsync(Guid userId,
    CancellationToken cancellationToken = default);
    }
}
