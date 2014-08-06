using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp
{
    class PhotoFileName
    {
        public String Width { get; set; }
        public string Height { get; set; }
        public String FrameImageName { get; set; }
        public String MatImageName { get; set; }

        public String ToString()
        {
            if (MatImageName.Trim() != "")
            {
                return this.Width + "_" + this.Height + "_" + this.FrameImageName + "_" + this.MatImageName;
            }
            return this.Width + "_" + this.Height + "_" + this.FrameImageName;
        }

    }
    public class PhotoFrameException : Exception
    {
        public PhotoFrameException(String message) :base(message){
            
         }

    }
    public class PhotoFrameImages
    {
        public  String ImageSource { get; set; }
        
        public PhotoFrameImageType ImageType { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public int Height { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public String FileName { get; set; }

        [System.ComponentModel.DefaultValue(0)]
        public int Width { get; set; }

        [System.ComponentModel.DefaultValue(PhotoUnit.Inches)]
        public PhotoUnit Unit { get; set; }
        public Color BorderColor { get; set; }
    }
    public enum  PhotoFrameImageType{
        MAT,FRAME,INNER
    }
    public enum  PhotoUnit
    {
        pixels, Inches
    }
    public class PhotoFileReturn
    {
      public String Filepath { get; set; }
      public String FileName { get; set; }
    }
    public class PhotoFrame
    {
        System.Drawing.Image Wood;
        System.Drawing.Graphics Woodcutter;
        
        private Pen FramPen,MatPen,ImagePen;
        
        private Image FrameImage;
        private int FrameImageWidth;
        private int FrameWidth;
        private int FrameHeight;
        

        private Image MatImage;
        private int MatImageWidth;
        
        private Image innerImage;
        private int InnerWidth;
        private int InnerHeight;

        private PhotoFileReturn ReturnFile;

        private PhotoFileName FileName;

        public PhotoFrame()
        {
            this.FileName = new PhotoFileName();
        }


        #region REGION1
        public void DevelopPhotoFrame(List<PhotoFrameImages> Images)
        {
            if (Images != null && Images.Count == 3)
            {
                foreach (PhotoFrameImages Image in Images)
                {
                        if (Image.ImageType == PhotoFrameImageType.FRAME) { createOuterFrame(Image); }
                        if (Image.ImageType == PhotoFrameImageType.INNER) { createInnerFrame(Image); }
                        if (Image.ImageType == PhotoFrameImageType.MAT) { createMatFrame(Image); }

                } Develop();
            }
            else
            {
                throw new PhotoFrameException("Please pass all PhotoFrameImageTypes in a List ");
            }
        }
        private void createOuterFrame(PhotoFrameImages Image1)
        {

            this.FramPen = new Pen(Image1.BorderColor, 0);
            this.FrameImage = Image1.ImageSource!=null && Image1.ImageSource!="" ?( File.Exists(Image1.ImageSource)?Image.FromFile(Image1.ImageSource):null ): null;
            this.FrameImageWidth = this.FrameImage != null ? this.FrameImage.Width : 0;
            this.FileName.FrameImageName = Image1.FileName;
        }
        private void createInnerFrame(PhotoFrameImages Image1)
        {
            this.ImagePen = new Pen(Image1.BorderColor, 0);
            this.innerImage = Image1.ImageSource!=null && Image1.ImageSource!="" ?( File.Exists(Image1.ImageSource)?Image.FromFile(Image1.ImageSource):null ): null;
            this.InnerHeight = Image1.Height;
            this.InnerWidth = Image1.Width;

            if (Image1.Unit == PhotoUnit.Inches) {
                this.InnerHeight *= this.InnerHeight;
                this.InnerWidth *= this.InnerWidth;
            }

            this.FileName.Height = Image1.Height.ToString();
            this.FileName.Width = Image1.Width.ToString();
        }
        private void createMatFrame(PhotoFrameImages Image1)
        {
            this.MatPen = new Pen(Image1.BorderColor, 0);
            this.MatImage = Image1.ImageSource != null && Image1.ImageSource != "" ? (File.Exists(Image1.ImageSource) ? Image.FromFile(Image1.ImageSource) : null) : null;
            this.MatImageWidth = this.MatImage != null ? this.MatImage.Width : 0;
            this.FileName.MatImageName = this.MatImage != null ? Image1.FileName : "";
        }
        private void Develop()
        {


            this.FrameWidth = (2 * this.FrameImageWidth + 2 * this.MatImageWidth + this.InnerWidth);
            this.FrameHeight = (2 *this.FrameImageWidth + 2 * this.MatImageWidth + this.InnerHeight);

            this.Wood = new Bitmap(Convert.ToInt16(FrameWidth), Convert.ToInt16(FrameHeight));
            this.Woodcutter = System.Drawing.Graphics.FromImage(this.Wood);

            this.drawFrames(FrameWidth, FrameHeight, FrameImageWidth, MatImageWidth, InnerWidth, InnerHeight);
        }
        #endregion


        #region REGION2
        public void DevelopPhotoFrame(String FrameImageSource,String MatImageSource,int ImageWidth,int ImageHeight,PhotoUnit Unit,String FrameImageFileName,String MatImageFileName)
        {
            if (File.Exists(FrameImageSource))
            {

                this.FramPen = new Pen(Color.Transparent, 0);
                this.MatPen = new Pen(Color.Transparent, 0);
                this.ImagePen = new Pen(Color.Transparent, 0);

                this.FrameImage = (FrameImageSource != null && FrameImageSource.Trim() != "") ? Image.FromFile(FrameImageSource) : null;
                this.MatImage = (MatImageSource != null && MatImageSource.Trim() != "") ? (File.Exists(MatImageSource)?Image.FromFile(MatImageSource) :null) : null;

                this.FrameImageWidth = (this.FrameImage != null) ?this.FrameImage.Size.Width : 0;
                this.MatImageWidth = (this.MatImage!=null) ? this.MatImage.Size.Width : 0;

                this.FileName.Height = ImageWidth.ToString();
                this.FileName.Width = ImageHeight.ToString();
                this.FileName.MatImageName = MatImageFileName;
                this.FileName.FrameImageName = FrameImageFileName;

                if (Unit == PhotoUnit.Inches)
                {
                    ImageWidth = 72 * ImageWidth;
                    ImageHeight = 72 * ImageHeight;
                }
                this.InnerHeight = ImageHeight;
                this.InnerWidth = ImageWidth;

                this.FrameWidth = (2 * FrameImageWidth + 2 * MatImageWidth + ImageWidth);
                this.FrameHeight = (2 * FrameImageWidth + 2 * MatImageWidth + ImageHeight);

               
                this.Wood = new Bitmap(Convert.ToInt32(FrameWidth), Convert.ToInt32(FrameHeight));
                this.Woodcutter = System.Drawing.Graphics.FromImage(this.Wood);

                this.drawFrames(FrameWidth, FrameHeight, FrameImageWidth, MatImageWidth, ImageWidth, ImageHeight);

                
            }
            else
            {
                throw new PhotoFrameException("Frame Image Source Doesn't Exist");
            }

              
        }
        #endregion



        private void drawFrames(int FrameWidth, int FrameHeight, int FrameImageWidth,int MatImageWidth,int ImageWidth,int ImageHeight)
        {

            int cal1 = FrameWidth - FrameImageWidth;
            int cal2 = FrameHeight - FrameImageWidth;
            int cal3 = FrameHeight - FrameImageWidth;
            int cal4 = FrameImageWidth + MatImageWidth;
            int cal5 = FrameImageWidth + MatImageWidth + ImageWidth;
            int cal6 = FrameImageWidth + MatImageWidth + ImageHeight;
       
            
            Point point1 = new Point(0, 0);
            Point point2 = new Point(FrameImageWidth, FrameImageWidth);
            Point point3 = new Point(FrameWidth - FrameImageWidth, FrameImageWidth);
            Point point4 = new Point(FrameWidth, 0);
            Point point5 = new Point(FrameWidth - FrameImageWidth, FrameHeight - FrameImageWidth);
            Point point6 = new Point(FrameWidth, FrameHeight);
            Point point7 = new Point(FrameImageWidth, FrameHeight - FrameImageWidth);
            Point point8 = new Point(0, FrameHeight);

            Point point9 = new Point(FrameImageWidth+MatImageWidth,FrameImageWidth+MatImageWidth);
            Point point10 = new Point(FrameImageWidth + MatImageWidth + ImageWidth, FrameImageWidth + MatImageWidth);
            Point point11 = new Point(FrameImageWidth + MatImageWidth + ImageWidth, FrameImageWidth + MatImageWidth + ImageHeight);
            Point point12 = new Point(FrameImageWidth + MatImageWidth, FrameImageWidth + MatImageWidth+ImageHeight);

            if (this.FrameImage != null)
            {
                this.drawOuterFrame(point1, point2, point3, point4, point5, point6, point7, point8, point9, point10, point11, point12);
            }
            this.drawInnerImageFrame(point9,point10,point11,point12);
            if (this.MatImage != null)
            {
                this.drawMatFrame(point1, point2, point3, point4, point5, point6, point7, point8, point9, point10, point11, point12);
            }
            this.Woodcutter.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            this.Woodcutter.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.Woodcutter.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            this.Woodcutter.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;


        }
        private void drawOuterFrame(Point point1,Point point2,Point point3,Point point4,Point point5,Point point6,Point point7,Point point8,Point point9,Point point10,Point point11,Point point12)
        {
       
  
            Point[] BottomcurvePoints = { 
                 point1,
                 point4,
                 point3,
                 point2
             };
            Point[] RightcurvePoints =
             {
                 point4,
                 point6,
                 point5,
                 point3
             };
            Point[] TopcurvePoints =
             {
                 point6,
                 point8,
                 point7,
                 point5
             };

            Point[] LeftcurvePoints =
             {
                 point8,
                 point1,
                 point2,
                 point7
             };

            this.Woodcutter.DrawPolygon(this.FramPen, TopcurvePoints);
            this.Woodcutter.DrawPolygon(this.FramPen, RightcurvePoints);
            this.Woodcutter.DrawPolygon(this.FramPen, LeftcurvePoints);
            this.Woodcutter.DrawPolygon(this.FramPen, BottomcurvePoints);


            TextureBrush FrameLeft=new TextureBrush(this.FrameImage);
            this.Woodcutter.FillPolygon(FrameLeft, LeftcurvePoints);


            TextureBrush FrameTop = new TextureBrush(this.FrameImage);
            
            FrameTop.TranslateTransform(0, 0);
            FrameTop.RotateTransform(90);
            
            this.Woodcutter.FillPolygon(FrameTop, BottomcurvePoints);

            TextureBrush FrameRight = new TextureBrush(this.FrameImage);
            FrameRight.TranslateTransform(this.FrameWidth - this.FrameImageWidth, 0);
            FrameRight.RotateTransform(180);
            this.Woodcutter.FillPolygon(FrameRight, RightcurvePoints);

            TextureBrush FrameBottom = new TextureBrush(this.FrameImage);
            FrameBottom.TranslateTransform(this.FrameImageWidth,this.FrameHeight);
            FrameBottom.RotateTransform(270);
            
            this.Woodcutter.FillPolygon(FrameBottom, TopcurvePoints);

        }
        private void drawMatFrame(Point point1, Point point2, Point point3, Point point4, Point point5, Point point6, Point point7, Point point8, Point point9, Point point10, Point point11, Point point12)
        {
            Point[] BottomcurvePoints =
             {
                 point2,
                 point3,
                 point10,
                 point9
             };
            Point[] RightcurvePoints =
             {
                 point3,
                 point5,
                 point11,
                 point10
             };
            Point[] TopcurvePoints =
             {
                 point5,
                 point7,
                 point12,
                 point11
             };

            Point[] LeftcurvePoints =
             {
                 point7,
                 point2,
                 point9,
                 point12
             };

            //Bottom
            this.Woodcutter.DrawPolygon(this.MatPen, TopcurvePoints);
            //Left
            this.Woodcutter.DrawPolygon(this.MatPen, LeftcurvePoints);
            //Right
            this.Woodcutter.DrawPolygon(this.MatPen, RightcurvePoints);
            //Top
            this.Woodcutter.DrawPolygon(this.MatPen, BottomcurvePoints);


            TextureBrush Topbrush = new TextureBrush(this.MatImage);
            Topbrush.TranslateTransform(this.FrameImageWidth,this.FrameImageWidth);

            TextureBrush Bottombrush = new TextureBrush(this.MatImage);
            Bottombrush.TranslateTransform(this.FrameImageWidth,this.FrameHeight-this.FrameImageWidth);
            Bottombrush.RotateTransform(180);
            this.Woodcutter.FillPolygon(Bottombrush, TopcurvePoints);

            TextureBrush Rightbrush = new TextureBrush(this.MatImage);
            Rightbrush.TranslateTransform(this.FrameWidth-this.FrameImageWidth-this.MatImage.Width,this.FrameImageWidth);
            Rightbrush.RotateTransform(90);

            this.Woodcutter.FillPolygon(Rightbrush, RightcurvePoints);
            this.Woodcutter.FillPolygon(Topbrush, BottomcurvePoints);

            TextureBrush Leftbrush = new TextureBrush(this.MatImage);
            Leftbrush.TranslateTransform(this.FrameImageWidth, this.FrameImageWidth);
            Leftbrush.RotateTransform(90);
            Leftbrush.RotateTransform(180);
            this.Woodcutter.FillPolygon(Leftbrush, LeftcurvePoints);

        }
        private void drawInnerImageFrame(Point point1, Point point2, Point point3, Point point4)
        {
            Point[] curvePoints4 =
             {
                 point1,
                 point2,
                 point3,
                 point4
             };

            this.Woodcutter.DrawPolygon(this.ImagePen, curvePoints4);
            TextureBrush brush;
            if (this.innerImage != null)
            {
                Image i =this.innerImage;
                brush = new TextureBrush(i, System.Drawing.Drawing2D.WrapMode.Clamp);
            }
            else
            {
                Bitmap Im = new Bitmap(this.InnerWidth, this.InnerHeight);
                brush = new TextureBrush((Image)Im);
            }
            brush.TranslateTransform(point1.X, point1.Y);
            this.Woodcutter.FillPolygon(brush, curvePoints4);
        }
        public PhotoFileReturn SaveImageAs(String RootDirectoryFolder)
        {
            String RootDirectory = "";
            if (RootDirectoryFolder != "")
            {
               RootDirectory = HttpContext.Current.Server.MapPath("~/" + RootDirectoryFolder);
               if (!Directory.Exists(RootDirectory))
               {
                   Directory.CreateDirectory(RootDirectory);
               }
            }
            var file = Path.Combine(RootDirectory + "/", this.FileName.ToString()+".png");
            if (!File.Exists(file))
            {
                 this.Wood.Save(file);
            }

            if (File.Exists(file))
            {
                ReturnFile = new PhotoFileReturn();
                ReturnFile.FileName = this.FileName.ToString();
                ReturnFile.Filepath = file;
                return ReturnFile;
            }
            return null;
        }

    }

}
