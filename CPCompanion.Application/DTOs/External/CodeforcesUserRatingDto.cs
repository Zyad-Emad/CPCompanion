using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPCompanion.Application.DTOs.External
{
    public class CodeforcesUserRatingDto
    {
        public bool Exists { get; set; }
        public int CurrentRating { get; set; }
        public int MaxRating { get; set; }
    }
}
