using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace QSharp
{
    public class QColor
    {
        public string name { get; set; }

        public string Hex { get; set; }
        public string lightHex { get; set; }
        public string darkHex { get; set; }
        public QColor()
        {
            defaultColor();
        }

        public QColor(string name)
        {
            switch (name)
            {
                case "red":
                    redColor();
                    break;
                case "orange":
                    orangeColor();
                    break;
                case "yellow":
                    yellowColor();
                    break;
                case "green":
                    greenColor();
                    break;
                case "blue":
                    blueColor();
                    break;
                case "indigo":
                    indigoColor();
                    break;
                case "purple":
                    purpleColor();
                    break;
                default:
                    defaultColor();
                    break;
            }
        } 

        private void defaultColor()
        {
            name = "default";
            lightHex = "#ACAAB2";
            Hex = "#8F8A99";
            darkHex = "#77737F";
        }

        private void redColor()
        {
            name = "red";
            lightHex = "#FC563C";
            Hex = "#FC363B";
            darkHex = "#E31C24";
        }
        private void orangeColor()
        {
            name = "orange";
            lightHex = "#FFAA00";
            Hex = "#FF9500";
            darkHex = "#FF6A00";
        }
        private void yellowColor ()
        {
            name = "yellow";
            lightHex = "#FFF480";
            Hex = "#F7E519";
            darkHex = "#FFD500";
        }
        private void greenColor()
        {
            name = "green";
            lightHex = "#17E639";
            Hex = "#00CC22";
            darkHex = "#00B300";
        }

        private void blueColor()
        {
            name = "blue";
            lightHex = "#5C73E6";
            Hex = "#415AD9";
            darkHex = "#2944CC";
        }
        private void indigoColor()
        {
            name = "purple";
            lightHex = "#3F388C";
            Hex = "#5A5499";
            darkHex = "#6262B3";
        }
        private void purpleColor()
        {
            name = "purple";
            lightHex = "#B500D9";
            Hex = "#9500B3";
            darkHex = "#730099";
        }
        
    }
}
