using System.Collections.Generic;
using QControlKit.Constants;

namespace QControlKit.Cue
{
    public class QTextCue : QCue
    {

        public int fixedWidth
        {
            get
            {
                return (int)propertyForKey(QOSCKey.FixedWidth);
            }
            set
            {
                if(fixedWidth  > 0)
                {
                    setProperty(value, QOSCKey.FixedWidth);
                }
            }
        }
        public string text
        {
            get
            {
                return propertyForKey(QOSCKey.Text).ToString();
            }

            set
            {
                setProperty(value, QOSCKey.Text);
            }
        }

        public string liveText
        {
            get
            {
                return propertyForKey(QOSCKey.LiveText).ToString();
            }

            set
            {
                setProperty(value, QOSCKey.LiveText);
            }
        }

        //TODO: 
        /* https://qlab.app/docs/v4/scripting/osc-dictionary-v4/#text-cue-methods
         * /cue/{cue_number}/text/format {json_string}
         * /cue/{cue_number}/text/format/alignment {alignment}
         * /cue/{cue_number}/text/format/fontFamily
         * /cue/{cue_number}/text/format/fontStyle
         * /cue/{cue_number}/text/format/fontFamilyAndStyle {family} {style}
         * /cue/{cue_number}/text/format/fontName {name}
         * /cue/{cue_number}/text/format/fontSize {number}
         * /cue/{cue_number}/text/format/lineSpacing {number}
         * /cue/{cue_number}/text/format/color {red} {green} {blue} {alpha}
         * /cue/{cue_number}/text/format/backgroundColor {red} {green} {blue} {alpha}
         * /cue/{cue_number}/text/format/strikethroughColor {red} {green} {blue} {alpha}
         * /cue/{cue_number}/text/format/underlineColor {red} {green} {blue} {alpha}
         * /cue/{cue_number}/text/format/strikethroughStyle {style}
         * /cue/{cue_number}/text/format/underlineStyle {style}
         * /cue/{cue_number}/text/outputSize
         * /cue/{cue_number}/liveText/outputSize
         */
    }
}
