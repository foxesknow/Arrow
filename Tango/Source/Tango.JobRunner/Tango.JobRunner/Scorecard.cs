using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    public sealed class Scorecard
    {
        private readonly List<Score> m_Scores = new();

        public Scorecard(Group group)
        {
            if(group is null) throw new ArgumentNullException(nameof(group));

            this.Group = group;
        }

        public DateTime StartUtc{get; internal set;}
        
        public DateTime StopUtc{get; internal set;}

        public Group Group{get;}

        public IReadOnlyList<Score> Scores
        {
            get{return m_Scores;}
        }

        public void Add(Score score)
        {
            if(score is null) throw new ArgumentNullException(nameof(score));

            m_Scores.Add(score);
        }
    }
}
