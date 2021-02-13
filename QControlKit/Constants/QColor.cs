namespace QControlKit.Constants
{
    public class QColor
    {
        public string name { get; set; }
        public string Hex { get; set; }
        public string lightHex { get; set; }
        public string darkHex { get; set; }

        public QColor()
        {
            noneColor();
        }

        public QColor(string name)
        {
            switch (name)
            {
                case QColorName.Red:
                    redColor();
                    break;
                case QColorName.Orange:
                    orangeColor();
                    break;
                case QColorName.Yellow:
                    yellowColor();
                    break;
                case QColorName.Green:
                    greenColor();
                    break;
                case QColorName.Blue:
                    blueColor();
                    break;
                case QColorName.Indigo:
                    indigoColor();
                    break;
                case QColorName.Purple:
                    purpleColor();
                    break;
                case QColorName.None:
                    noneColor();
                    break;
                default:
                    defaultColor();
                    break;
            }
        } 

        private void defaultColor()
        {
            name = QColorName.Default;
            lightHex = "#ACAAB2";
            Hex = "#8F8A99";
            darkHex = "#77737F";
        }

        private void noneColor()
        {
            name = QColorName.None;
            lightHex = "#ACAAB2";
            Hex = "#8F8A99";
            darkHex = "#77737F";
        }

        private void redColor()
        {
            name = QColorName.Red;
            lightHex = "#FC563C";
            Hex = "#FC363B";
            darkHex = "#E31C24";
        }

        private void orangeColor()
        {
            name = QColorName.Orange;
            lightHex = "#FFAA00";
            Hex = "#FF9500";
            darkHex = "#FF6A00";
        }

        private void yellowColor ()
        {
            name = QColorName.Yellow;
            lightHex = "#FFF480";
            Hex = "#F7E519";
            darkHex = "#FFD500";
        }

        private void greenColor()
        {
            name = QColorName.Green;
            lightHex = "#17E639";
            Hex = "#00CC22";
            darkHex = "#00B300";
        }

        private void blueColor()
        {
            name = QColorName.Blue;
            lightHex = "#5C73E6";
            Hex = "#415AD9";
            darkHex = "#2944CC";
        }

        private void indigoColor()
        {
            name = QColorName.Indigo;
            lightHex = "#3F388C";
            Hex = "#5A5499";
            darkHex = "#6262B3";
        }

        private void purpleColor()
        {
            name = QColorName.Purple;
            lightHex = "#B500D9";
            Hex = "#9500B3";
            darkHex = "#730099";
        }
        
    }
}
