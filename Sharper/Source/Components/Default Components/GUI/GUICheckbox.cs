using Microsoft.Xna.Framework;
using Sharper.ECS;

namespace Sharper
{
    public class GUICheckbox : Component
    {
        //Parameterless constructor is needed for every component
        public GUICheckbox()
        {

        }
        public bool isChecked;
        public string uncheckedTexName = "Checkbox";
        public string checkedTexName = "CheckboxTrue";
    }
}
