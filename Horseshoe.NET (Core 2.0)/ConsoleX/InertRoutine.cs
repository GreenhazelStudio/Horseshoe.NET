using System;
using System.Collections.Generic;
using System.Security;
using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ConsoleX
{
    public class InertRoutine : Routine
    {
        public override Title Title { get; }

        public override bool RenderTitleOnRun => false;

        public override Title MenuTitle => null;

        public override bool AutoAppendExitRoutineMenuItem => false;

        public override bool ContinuousDisplay => false;

        public override int SpacesAfterLoop => 0;

        public InertRoutine(string title)
        {
            Title = title;
        }
    }
}
