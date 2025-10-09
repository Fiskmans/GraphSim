using Godot;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim.Ui
{
    public partial class BuildButton : PopupMenu
    {
        [Export]
        Node Target;

        public override void _Ready()
        {
            base._Ready();

            if (Target == null || !(Target is Site))
            {
                GD.PrintErr("Invalid target node");
                QueueFree();
                return;
            }

            this.AddRange(this.GetFirstParentOfType<DataLoader>().Data.Buildings, (b) => b.Name, (b) =>
            {
                (Target as Site).Construction = new Construction(b);
            });
        }
    }
}
