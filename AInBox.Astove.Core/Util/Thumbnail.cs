using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace AInBox.Astove.Core.Util
{
    public class Thumbnail
    {
        /// <summary>
        /// Retorna os tipos suportados
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatFromFileName(string fileName)
        {
            if (fileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase))
                return ImageFormat.Jpeg;

            if (fileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase))
                return ImageFormat.Bmp;

            if (fileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase))
                return ImageFormat.Gif;

            if (fileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase))
                return ImageFormat.Png;

            return ImageFormat.Jpeg;
        }

        public static string GetContentType(ImageFormat imageFormat)
        {
            if (imageFormat == ImageFormat.Bmp)
                return "image/bmp";

            if (imageFormat == ImageFormat.Jpeg)
                return "image/jpeg";

            if (imageFormat == ImageFormat.Png)
                return "image/png";

            if (imageFormat == ImageFormat.Gif)
                return "image/gif";

            return "image/jpeg";
        }

        public static byte[] CreateThumbnail(byte[] bytes, int width, int height, long quality, int? maxSize)
        {
            if (quality < 1)
                throw new ArgumentOutOfRangeException("Quality must be greater than 0.");
            else if (quality > 100)
                quality = 100L;

            Image bmp = CreateBitmapThumbnail(bytes, width, height, maxSize);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();

            ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters Params = new EncoderParameters(1);
            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            bmp.Save(stream, Info[1], Params);
            bmp.Dispose();

            return stream.ToArray();
        }

        protected static Bitmap CreateBitmapThumbnail(byte[] bytes, int lnWidth, int lnHeight, int? maxSize)
        {
            Bitmap bmpOut = null;

            try
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
                Bitmap loBMP = new Bitmap(stream);
                ImageFormat loFormat = loBMP.RawFormat;

                decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //Largura ou Altura maior que 1024px
                if (lnWidth == 0 && lnHeight == 0 && maxSize.HasValue && (loBMP.Width > maxSize.Value || loBMP.Height > maxSize.Value))
                {
                    if (loBMP.Width > loBMP.Height)
                    {
                        lnWidth = maxSize.Value;
                        lnHeight = 0;
                    }
                    else
                    {
                        lnWidth = 0;
                        lnHeight = maxSize.Value;
                    }
                }

                //Nenhuma altura ou comprimento informado -> tamanho real da imagem
                if (lnWidth == 0 && lnHeight == 0)
                {
                    lnWidth = int.MaxValue;
                    lnHeight = int.MaxValue;
                }

                //*** If the image is smaller than a thumbnail just return it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;

                //Casos com alguma dimensão fixada
                if (lnWidth == 0 || lnHeight == 0)
                {
                    if (lnHeight == 0)
                    {
                        lnRatio = (decimal)lnWidth / loBMP.Width;
                        lnNewWidth = lnWidth;
                        decimal lnTemp = loBMP.Height * lnRatio;
                        lnNewHeight = (int)lnTemp;
                    }
                    else
                    {
                        lnRatio = (decimal)lnHeight / loBMP.Height;
                        lnNewHeight = lnHeight;
                        decimal lnTemp = loBMP.Width * lnRatio;
                        lnNewWidth = (int)lnTemp;
                    }

                }
                else
                {
                    //Dimensoes variáveis
                    if (loBMP.Width > loBMP.Height)
                    {
                        lnRatio = (decimal)lnWidth / loBMP.Width;
                        lnNewWidth = lnWidth;
                        decimal lnTemp = loBMP.Height * lnRatio;
                        lnNewHeight = (int)lnTemp;
                    }
                    else
                    {
                        lnRatio = (decimal)lnHeight / loBMP.Height;
                        lnNewHeight = lnHeight;
                        decimal lnTemp = loBMP.Width * lnRatio;
                        lnNewWidth = (int)lnTemp;
                    }
                }

                if (lnNewWidth == 0)
                    lnNewWidth = 1;

                if (lnNewHeight == 0)
                    lnNewHeight = 1;

                bmpOut = new Bitmap(lnNewWidth, lnNewHeight, PixelFormat.Format24bppRgb);
                bmpOut.SetResolution(72, 72);

                using (Graphics g = Graphics.FromImage(bmpOut))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.FillRectangle(Brushes.Transparent, 0, 0, lnNewWidth, lnNewHeight);
                    g.DrawImage(loBMP, new Rectangle(0, 0, lnNewWidth, lnNewHeight));
                }
                loBMP.Dispose();
            }
            catch
            {
                return null;
            }

            return bmpOut;
        }

        public static byte[] CreateCrop(byte[] bytes, int width, int height, bool isForWeb)
        {
            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(bytes));

            byte[] original = new byte[bytes.Length];
            bytes.CopyTo(original, 0);

            if (img.Height > img.Width) // imagem em pé
            {
                bytes = Thumbnail.CreateThumbnail(bytes, width, 0, 100L, null);
                img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(bytes));

                if (img.Height < height)
                {
                    bytes = Thumbnail.CreateThumbnail(original, 0, height, 100L, null);
                    img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(bytes));
                }
            }
            else // imagem deitada
            {
                bytes = Thumbnail.CreateThumbnail(bytes, 0, height, 100L, null);
                img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(bytes));

                if (img.Width < width)
                {
                    bytes = Thumbnail.CreateThumbnail(original, width, 0, 100L, null);
                    img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(bytes));
                }
            }

            img = System.Drawing.Image.FromStream(new MemoryStream(bytes));

            int x = (img.Width - width) / 2;
            x = (x < 0) ? 0 : x;

            int y = (img.Height - height) / 2;
            y = (y < 0) ? 0 : y;

            return CreateCrop(bytes, width, height, x, y, isForWeb);
        }

        public static byte[] CreateCrop(byte[] bytes, int width, int height, int x, int y, bool isForWeb)
        {
            if (width == 0 && height == 0)
                return bytes;

            Image imgPhoto = Image.FromStream(new MemoryStream(bytes));

            if (width == 0)
                width = 1;

            if (height == 0)
                height = 1;

            Bitmap bmPhoto = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            if (isForWeb)
                bmPhoto.SetResolution(72, 72);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.CompositingQuality = CompositingQuality.HighQuality;
            grPhoto.SmoothingMode = SmoothingMode.HighQuality;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;

            grPhoto.FillRectangle(Brushes.Transparent, 0, 0, width, height);
            grPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, width, height), x, y, width, height, GraphicsUnit.Pixel);

            MemoryStream mm = new MemoryStream();
            ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters Params = new EncoderParameters(1);
            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            bmPhoto.Save(mm, Info[1], Params);

            imgPhoto.Dispose();
            bmPhoto.Dispose();
            grPhoto.Dispose();

            return mm.ToArray();
        }


        public static byte[] CreateWaterMark(byte[] img, byte[] watMark)
        {
            Image image = Image.FromStream(new MemoryStream(img));
            Image waterMark = Image.FromStream(new MemoryStream(watMark));

            Image finalImage;
            finalImage = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            Graphics graphic = Graphics.FromImage(finalImage);
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);
            graphic.DrawImage(image, rectangle);

            Rectangle rectangleLogo = new Rectangle(finalImage.Width - waterMark.Width, finalImage.Height - waterMark.Height, waterMark.Width, waterMark.Height);
            graphic.DrawImage(waterMark, rectangleLogo);

            System.IO.MemoryStream finalStream = new MemoryStream();
            ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters Params = new EncoderParameters(1);
            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            finalImage.Save(finalStream, Info[1], Params);

            return finalStream.ToArray();
        }
    }
}