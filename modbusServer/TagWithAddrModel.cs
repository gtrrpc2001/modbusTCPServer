using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deviceTest
{
    public class TagWithAddrModel
    {
        public int idx;
        public int used;
        public string eq;
        public string group;
        public string Tag;
        public int Address;
        public double vmin;
        public double vmax;
        public double vsum;
        public double vcnt;
        public double vavg;
        public double scale;
        public double offset;

        public TagWithAddrModel(int idx, int used, string eq, string group, string Tag, int Address, double vmin = 0, double vmax = 0, double vsum = 0, double vcnt = 0, double vavg = 0, double scale = 0, double offset = 0)
        {
            this.idx = idx;
            this.used = used;
            this.eq = eq;
            this.group = group;
            this.Tag = Tag;
            this.Address = Address;
            this.vmin = vmin;
            this.vmax = vmax;
            this.vsum = vsum;
            this.vcnt = vcnt;
            this.vavg = vavg;
            this.scale = scale;
            this.offset = offset;
            
        }
    }
}
