//    This file is part of PlasmaWCET.
//
//    PlasmaWCET is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    PlasmaWCET is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with PlasmaWCET.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCET
{
    public class Cache : WCET.ICache
    {
        public uint MissCount { get; set; }
        public uint MissOverhead { get; set; }
        public int[] Tags { get; private set; }
        public Cache(uint missOverhead)
        {
            MissOverhead = missOverhead;
            Tags = new int[1024];
            for (int i = 0; i < Tags.Length; i++)
                Tags[i] = -1;
        }
        public uint Read(uint Address)
        {
            var index = (Address >> 2) & 0x3ff;
            var tag = (Address >> 12) & 0x1FF;
            var oldTag = Tags[index];
            Tags[index] = (int)tag;
            if (tag != oldTag)
                MissCount++;
            return tag == oldTag ? 0 : MissOverhead;
        }
        public Cache Clone()
        {
            var cache = new Cache(MissOverhead);
            for (int i = 0; i < cache.Tags.Length; i++)
                cache.Tags[i] = Tags[i];
            cache.MissCount = MissCount;
            return cache;
        }
    }
}
