using Game;
using System;
using System.Collections.Generic;
using System.Text;
using TemplatesDatabase;

namespace CreatorModAPI
{
    public class CreatorModAPIEditPaletteDialog : EditPaletteDialog
    {
        private ButtonWidget okButton;
        public CreatorModAPIEditPaletteDialog(WorldPalette palette) : base(palette)
        {
            this.okButton = this.Children.Find<ButtonWidget>("EditPaletteDialog.OK", true);
        }

        public override void Update()
        {
            base.Update();
            if (okButton.IsClicked)
            {
                GameManager.Project.FindSubsystem<SubsystemPalette>().Load(new ValuesDictionary());
            }
        }
    }
}
