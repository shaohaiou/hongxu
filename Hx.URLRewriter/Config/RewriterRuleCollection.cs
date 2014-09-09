using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Hx.URLRewriter.Config
{
    [Serializable()]
    public class RewriterRuleCollection : CollectionBase
    {
        /// <summary>
        /// Adds a new RewriterRule to the collection.
        /// </summary>
        /// <param name="r">A RewriterRule instance.</param>
        public virtual void Add(RewriterRule r)
        {
            this.InnerList.Add(r);
        }

        public virtual void AddRange(RewriterRuleCollection rs)
        {
            this.InnerList.AddRange(rs);
        }

        /// <summary>
        /// Gets or sets a RewriterRule at a specified ordinal index.
        /// </summary>
        public RewriterRule this[int index]
        {
            get
            {
                return (RewriterRule)this.InnerList[index];
            }
            set
            {
                this.InnerList[index] = value;
            }
        }
    }
}
