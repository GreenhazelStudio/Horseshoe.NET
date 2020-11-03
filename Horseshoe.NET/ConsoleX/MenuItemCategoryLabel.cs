using System;
using System.Collections.Generic;
using System.Security;
using Horseshoe.NET.Collections;

namespace Horseshoe.NET.ConsoleX
{
    public class MenuItemCategoryLabel : Routine
    {
        public override Title Title { get; }

        public override RoutineTitleRenderPolicy RenderTitlePolicy => RoutineTitleRenderPolicy.NoRender;

        public override Title MenuTitle => null;

        public override bool AutoAppendExitRoutineMenuItem => false;

        public override bool ContinuousDisplay => false;

        public override int SpacesAfterLoop => 0;

        public MenuItemCategoryLabel(string title)
        {
            Title = title;
        }

        public override void Run()
        {
        }
    }
}
