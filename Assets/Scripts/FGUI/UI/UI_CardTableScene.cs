/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace UI
{
    public partial class UI_CardTableScene : GComponent
    {
        public GList m_playerZone;
        public GList m_aiZone;
        public GList m_shareCardZone;
        public GTextField m_resultTxt;
        public GButton m_compareBtn;
        public GButton m_giveUpBtn;
        public GButton m_dealBtn;
        public const string URL = "ui://uctenneh9l5k1";

        public static UI_CardTableScene CreateInstance()
        {
            return (UI_CardTableScene)UIPackage.CreateObject("UI", "CardTableScene");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            m_playerZone = (GList)GetChildAt(1);
            m_aiZone = (GList)GetChildAt(2);
            m_shareCardZone = (GList)GetChildAt(3);
            m_resultTxt = (GTextField)GetChildAt(4);
            m_compareBtn = (GButton)GetChildAt(5);
            m_giveUpBtn = (GButton)GetChildAt(6);
            m_dealBtn = (GButton)GetChildAt(7);
        }
    }
}