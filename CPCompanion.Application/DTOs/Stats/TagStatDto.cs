using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.Stats
{
    public class TagStatDto
    {
        public string Tag { get; set; } = default!;
        public int SolvedCount { get; set; }
    }
}
