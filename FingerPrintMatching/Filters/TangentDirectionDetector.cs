using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;

namespace FingerPrintMatching.Filters
{
    public class TangentDirectionDetector : AForge.Imaging.Filters.BaseFilter
    {
        private short[,] _mask;
        private TangentDirectionDetectorMaskType _type;
        private int _masksize;
        public short[,] Mask { get { return _mask; } set { _mask = value; } }
        public TangentDirectionDetectorMaskType Type { get { return _type; } set { _type = value; } }
        public int MaskSize { get { return _masksize; } set { _masksize = value; } }

        public static short[,] GetTangentDirectionDetectorMask(TangentDirectionDetectorMaskType type, int masksize)
        {
            if (masksize % 2 == 0 || masksize < 3)
                throw new ArgumentException("Only odd values >= 3 for mask size are allowed - Value passed " + masksize);
            short[,] mask = new short[masksize, masksize];
            for (int i = 0; i < masksize; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < masksize; j++) mask[i, j] = 1;
                }
            }
            int centre = masksize / 2;
            short[][] circles = new short[centre + 1][];
            for (int i = 0; i <= centre; i++)
            {
                int c = 0, j;
                int size = 4 * (masksize - 2 * i - 1);
                size = (size == 0) ? 1 : size;
                circles[i] = new short[size];
                for (j = i; j < masksize - i - 1; j++)
                    circles[i][c++] = mask[i, j];
                for (j = i; j < masksize - i - 1; j++)
                    circles[i][c++] = mask[j, masksize - i - 1];
                for (j = masksize - i - 1; j > i; j--)
                    circles[i][c++] = mask[masksize - i - 1, j];
                for (j = masksize - i - 1; j > i; j--)
                    circles[i][c++] = mask[j, i];
            }
            double div = Math.PI * ((int)type * 1.0 / (int)TangentDirectionDetectorMaskType.PI);
            double tan = Math.Tan(div);
            if (tan >= 1)
                tan = 2 * Math.Cos(div / 2) / Math.Sin(div / 2);
            for (int i = 0; i <= centre; i++)
            {
                int shift = (int)((masksize / 2 - i + 1) * tan);
                var head = circles[i].Take(circles[i].Length - shift);
                var last = circles[i].Skip(circles[i].Length - shift);
                circles[i] = last.Concat(head).ToArray();
            }
            for (int i = 0; i <= centre; i++)
            {
                int c = 0, j;
                for (j = i; j < masksize - i - 1; j++)
                    mask[i, j] = circles[i][c++];
                for (j = i; j < masksize - i - 1; j++)
                    mask[j, masksize - i - 1] = circles[i][c++];
                for (j = masksize - i - 1; j > i; j--)
                    mask[masksize - i - 1, j] = circles[i][c++];
                for (j = masksize - i - 1; j > i; j--)
                    mask[j, i] = circles[i][c++];
            }
            return mask;
        }

        public TangentDirectionDetector()
        {
            _type = TangentDirectionDetectorMaskType.PI;
        }

        public override Dictionary<PixelFormat, PixelFormat> FormatTranslations
        {
            get
            {
                return new Dictionary<PixelFormat, PixelFormat>();
            }
        }

        protected override void ProcessFilter(UnmanagedImage sourceData, UnmanagedImage destinationData)
        {
            
        }

        public enum TangentDirectionDetectorMaskType
        {
            Zero = 0,
            PI1By8 = 6,
            PI1By6 = 8,
            PI1By4 = 12,
            PI1By3 = 16,
            PI3By8 = 18,
            PI1By2 = 24,
            PI5By8 = 30,
            PI2By3 = 32,
            PI3By4 = 36,
            PI5By6 = 40,
            PI7By8 = 42,
            PI = 48
        }
    }
}
