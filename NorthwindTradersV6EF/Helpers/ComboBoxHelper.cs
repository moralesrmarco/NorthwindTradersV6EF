using System;
using System.Data;
using System.Windows.Forms;
using Utilities;

namespace NorthwindTradersV6EF.Helpers
{
    internal class ComboBoxHelper
    {
        public static void LlenarCbo(ComboBox cbo, DataTable dt, string displayMember, string valueMember)
        { 
            cbo.DataSource = dt;
            cbo.DisplayMember = displayMember;
            cbo.ValueMember = valueMember;
            cbo.SelectedIndex = 0;
        }
    }
}
