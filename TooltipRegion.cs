using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphSim
{
    public partial class TooltipRegion : Godot.Control
    {

        private PanelContainer Popup = new PanelContainer
        {
            Visible = false
        };
        private Control Content;
        private SlotItem _TooltipOwner;
        private SlotItem TooltipOwner {
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


        public override void _EnterTree()
        {
            this.AddChild(Popup);
        }

        public void Show(Control content, SlotItem owner)
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
            Popup.Visible = false;
        }

        private void TooltipOwner_TreeExited()
        {
            Locked = false;
            Close(Content);
        }

        public void ToggleLock(Control content, SlotItem owner)
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
