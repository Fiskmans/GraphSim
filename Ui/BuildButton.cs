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

            GraphSim.Data.Data Data = this.GetFirstParentOfType<DataLoader>().Data;
            

            this.AddRange(Data.Buildings, (b) => b.Name, (b) =>
            {
                (Target as Site).Construction = new Construction(b);
            });
        }
    }
}
