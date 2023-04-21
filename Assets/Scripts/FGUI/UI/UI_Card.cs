/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI
{
    public partial class UI_Card : GComponent
    {
        public Controller m_cardTypeCtl;
        public Controller m_isShowCtl;
        public GTextField m_numTxt;
        public const string URL = "ui://uctennehotm60";

        public static UI_Card CreateInstance()
        {
            return (UI_Card)UIPackage.CreateObject("UI", "Card");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_cardTypeCtl = GetControllerAt(0);
            m_isShowCtl = GetControllerAt(1);
            m_numTxt = (GTextField)GetChildAt(2);
        }
    }
}