using Godot;
using GraphSim.Data;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class Construction
    {
        public Building Building;
        public Site Site;

        Vector2 GridPos;
        Vector2 offset;
        Vector2 _At;
        public Vector2 At 
        { 
            get => _At;
            set 
            {
                _At = value - offset;
                GridPos = Site.SnapToGrid(_At);
            }
        }

        public Construction(Building building)
        {
            Building = building;

            Rect2I bounds = new Rect2I();

            foreach (Rect2I rect in Building.Shape)
                bounds = bounds.Merge(rect);

            offset = (Vector2)bounds.Size * 0.5f * Constants.NodeSpacing;
        }

        public void Draw(Control onto)
        {
            Color gridColor = new Color(1, 1, 0, 0.2f);

            foreach (Rect2I rect in Building.Shape)
                onto.DrawRect(new Rect2(GridPos + (Vector2)rect.Position * Constants.NodeSpacing, (Vector2)rect.Size * Constants.NodeSpacing), gridColor);

            foreach (Rect2I rect in Building.Shape)
                onto.DrawRect(new Rect2(GridPos + (Vector2)rect.Position * Constants.NodeSpacing, (Vector2)rect.Size * Constants.NodeSpacing), new Color(1,1,1), filled:false);

            foreach (Rect2I rect in Building.Shape)
                onto.DrawRect(new Rect2(At + (Vector2)rect.Position * Constants.NodeSpacing, (Vector2)rect.Size * Constants.NodeSpacing), new Color(1,1,1, 0.3f), filled: false);
        }

        public ConstructionSite Build()
        {
            ConstructionSite consite = new ConstructionSite(Site.GridCoordsAt(GridPos), Building);
            return consite;
        }
    }
}
