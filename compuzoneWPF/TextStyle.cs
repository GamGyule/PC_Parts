using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace compuzoneWPF
{
    class TextStyle
    {
        private Style textRight = new Style();
        private Style textCenter = new Style();

        public TextStyle()
        {
            textRight.TargetType = typeof(DataGridCell);
            Setter setter1 = new Setter();
            setter1.Property = DataGridCell.HorizontalAlignmentProperty;
            setter1.Value = HorizontalAlignment.Right;
            textRight.Setters.Add(setter1);

            textCenter.TargetType = typeof(DataGridCell);
            Setter setter2 = new Setter();
            setter2.Property = DataGridCell.HorizontalAlignmentProperty;
            setter2.Value = HorizontalAlignment.Center;
            textCenter.Setters.Add(setter2);
        }

        public Style getTextRight()
        {
            return textRight;
        }

        public Style getTextCetner()
        {
            return textCenter;
        }
    }
}
