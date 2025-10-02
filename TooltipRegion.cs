using Godot;
using GraphSim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    [Tool]
    public partial class TooltipRegion : HBoxContainer
    {
        [Export]
        private PanelContainer Popup;

        private int _Width = 200;

        [Export]
        private int Width
        {
            get => _Width;
            set
            {
                _Width = value;
                if (Popup != null)
                    Popup.CustomMinimumSize = new Vector2(value, 0);
            }
        }

        private Control Content;
        private SiteItem _TooltipOwner;
        private SiteItem TooltipOwner {
            get => _TooltipOwner;
            set
            {
                if (_TooltipOwner != null)
                    _TooltipOwner.TreeExited -= TooltipOwner_TreeExited;

                _TooltipOwner = value;

                if (_TooltipOwner != null)
                    _TooltipOwner.TreeExited += TooltipOwner_TreeExited;
            }
        }
        private bool Locked = false;


        public override void _Ready()
        {
            Popup = new PanelContainer
            {
                CustomMinimumSize = new Vector2(Width, 0),
                Name = "TooltipArea"
            };
            
            AddChild(Popup);
        }

        public void Show(Control content, SiteItem owner)
        {
            if (Locked)
                return;

            if (Content != null)
                Close(Content);

            Popup.AddChild(content);
            Popup.ResetSize();
            Popup.Position = owner.GlobalPosition + owner.Size - GlobalPosition;
            Popup.Visible = true;
            Popup.MoveToFront();

            TooltipOwner = owner;
            TooltipOwner.ShowingTooltip = true;
            Content = content;
        }

        public void Close(Control content)
        {
            if (Locked)
                return;

            if (!object.ReferenceEquals(content, Content))
                return;

            Popup.RemoveChild(Content);

            TooltipOwner.ShowingTooltip = false;
            TooltipOwner = null;
            Content = null;
        }

        private void TooltipOwner_TreeExited()
        {
            Locked = false;
            Close(Content);
        }

        public void ToggleLock(Control content, SiteItem owner)
        {
            if (!object.ReferenceEquals(content, Content))
            {
                Locked = false;
                Show(content, owner);
                return;
            }

            Locked = !Locked;
        }
    }
}
