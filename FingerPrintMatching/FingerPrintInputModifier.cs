using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FingerPrintMatching
{
    public class FingerPrintInputModifier
    {
        public static Bitmap ModifyInputImage(Bitmap input, FingerPrintModifierOption option)
        {
            return Method4_Skeletonization(input);
        }

        private static Bitmap Method1(Bitmap input, FingerPrintModifierOption option)
        {
            ImageStatistics statistics = new ImageStatistics(input);
            if (!statistics.IsGrayscale)
            {
                input = Grayscale.CommonAlgorithms.BT709.Apply(input);
            }
            HistogramEqualization histEqFilter = new HistogramEqualization();
            Bitmap histEqImage = histEqFilter.Apply(input);
            OtsuThreshold th = new OtsuThreshold();
            Bitmap thImage = histEqImage;
            int ci = 0, cj = 0, dw = 32, dh = 32;
            for (int i = 0; i < thImage.Width; i += 32)
            {
                for (int j = 0; j < thImage.Height; j += 32)
                {
                    if (ci + 32 > thImage.Width) dw = thImage.Width - i * 32;
                    if (cj + 32 > thImage.Height) dh = thImage.Height - j * 32;
                    Rectangle localRegion = new Rectangle(i, j, dw, dh);
                    th.ApplyInPlace(thImage, localRegion);
                }
            }
            BilateralSmoothing bs = new BilateralSmoothing();
            bs.KernelSize = option.KernelSize;
            bs.ColorFactor = option.ColorFactor;
            return bs.Apply(thImage);
        }
        private static Bitmap Method2(Bitmap input, FingerPrintModifierOption option)
        {
            ImageStatistics statistics = new ImageStatistics(input);
            if (!statistics.IsGrayscale)
            {
                input = Grayscale.CommonAlgorithms.BT709.Apply(input);
            }
            //HistogramEqualization histEqFilter = new HistogramEqualization();
            //Bitmap histEqImage = histEqFilter.Apply(input);
            OtsuThreshold th = new OtsuThreshold();
            Bitmap thImage = input;
            int ci = 0, cj = 0, dw = 32, dh = 32;
            for (int i = 0; i < thImage.Width; i += 32)
            {
                for (int j = 0; j < thImage.Height; j += 32)
                {
                    if (ci + 32 > thImage.Width) dw = thImage.Width - i * 32;
                    if (cj + 32 > thImage.Height) dh = thImage.Height - j * 32;
                    Rectangle localRegion = new Rectangle(i, j, dw, dh);
                    th.ApplyInPlace(thImage, localRegion);
                }
            }
            BilateralSmoothing bs = new BilateralSmoothing();
            bs.KernelSize = option.KernelSize;
            bs.ColorFactor = option.ColorFactor;
            thImage = bs.Apply(thImage);
            return th.Apply(thImage);
        }
        private static Bitmap Method3_Sobel(Bitmap input)
        {
            CannyEdgeDetector ced = new CannyEdgeDetector();
            Bitmap b = ced.Apply(input);
            OtsuThreshold th = new OtsuThreshold();
            b = th.Apply(b);
            Invert i = new Invert();
            return i.Apply(b);
        }
        private static Bitmap FilterSequence1(Bitmap input)
        {
            // create filter sequence
            AForge.Imaging.Filters.FiltersSequence filterSequence =
                new AForge.Imaging.Filters.FiltersSequence();
            // add 8 thinning filters with different structuring elements
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            // create filter iterator for 10 iterations
            AForge.Imaging.Filters.FilterIterator filter =
                new AForge.Imaging.Filters.FilterIterator(filterSequence, 10);
            // apply the filter
            return filter.Apply(input);
        }
        private static Bitmap Method4_Skeletonization(Bitmap input)
        {
            input = new Invert().Apply(input);
            input = Method1(input, new FingerPrintModifierOption(15, 64));
            input = Method2(input, new FingerPrintModifierOption(3, 32));
            input = FilterSequence1(input);
            input = new Invert().Apply(input);
            input = new Erosion().Apply(input);
            input = new Erosion().Apply(input);
            Bitmap input2 = new BinaryDilatation3x3().Apply(input);
            input = new Add(input).Apply(input2);
            input = new SimpleSkeletonization().Apply(input);
            input = new Invert().Apply(input);
            input = new Erosion().Apply(input);
            input = new BinaryDilatation3x3().Apply(input);
            input = new Erosion().Apply(input);
            input = new Erosion().Apply(input);
            input = new BinaryDilatation3x3().Apply(input);
            input = new SimpleSkeletonization().Apply(input);
            input = new Invert().Apply(input);
            return input;
        }
    }
}
