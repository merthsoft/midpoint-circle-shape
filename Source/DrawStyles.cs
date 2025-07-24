using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Merthsoft.DesignatorShapes.MidpointCircle;

public class Filled : Verse.DrawStyle
{
    public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
    {
        buffer.Clear();
        buffer.AddRange(MidpointCircle.Filled(origin, target));
    }
}

public class Outline : Verse.DrawStyle
{
    public override void Update(IntVec3 origin, IntVec3 target, List<IntVec3> buffer)
    {
        buffer.Clear();
        buffer.AddRange(MidpointCircle.Outline(origin, target));
    }
}