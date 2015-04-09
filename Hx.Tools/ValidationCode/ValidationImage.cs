using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Hx.Tools.ValidationCode
{
    public class ValidationImage
    {
        Bitmap bmp;
        const int ww = 12;
        const int hh = 13;
        public ValidationImage(Bitmap bmp)
        {
            this.bmp = bmp;
        }

        public List<double> test()
        {
            Bitmap temp = Normalized(bmp);
            return this.GetPixelCollection(temp);
        }

        public List<List<double>> Operate()
        {
            ConvertToGray();
            List<int> vertical = GetVerticalSpilterLine(this.bmp);
            List<List<double>> resutl = new List<List<double>>();
            if (vertical.Count != 8) return resutl;
            int i, j;
            j = 0;
            for (i = 0; i < 8; i++)
            {
                if (i % 2 > 0)
                {
                    Bitmap temp = null;
                    Rectangle rec;
                    try
                    {
                        //垂直分割 4-1=2
                        rec = new Rectangle(vertical[i - 1] + 1, 0, vertical[i] - vertical[i - 1] - 1, 20);
                        temp = (Bitmap)bmp.Clone(rec, bmp.PixelFormat);

                        List<int> level = GetLevelSpilterLine(temp);

                        int begin = level[0];
                        int end;
                        if (begin > 10)//表示数字的下边没有空白线
                            end = begin;
                        else
                        {
                            if (level.Count == 2)
                                end = level[1] - begin;
                            else
                                end = 20 - begin;
                        }

                        rec = new Rectangle(0, begin + 1, temp.Width, end - 1);
                        temp = (Bitmap)temp.Clone(rec, temp.PixelFormat);

                        temp = Normalized(temp);
                        resutl.Add(GetPixelCollection(temp));

                        ++j;
                    }
                    finally
                    {
                        temp.Dispose();
                    }
                }
            }
            return resutl;
        }

        /// <summary>
        /// 获得灰度图像素集合
        /// 0表示白色1表示黑色
        /// </summary>
        /// <param name="bmp">待处理的图片</param>
        /// <returns>像素集合</returns>
        List<double> GetPixelCollection(Bitmap temp)
        {
            List<double> result = new List<double>();

            for (int h = 0; h < temp.Height; h++)
            {
                for (int w = 0; w < temp.Width; w++)
                {
                    Color c = temp.GetPixel(w, h);
                    int r = Convert.ToInt32(c.R);
                    if (r == 0)
                        result.Add(1);
                    else
                        result.Add(0);
                }
            }
            return result;
        }
        /// <summary>
        /// 把图片的宽高统一，，行话叫归一
        /// </summary>
        /// <param name="bitmap">需要处理的图片</param>
        Bitmap Normalized(Bitmap bitmap)
        {
            Bitmap temp = new Bitmap(ww, hh);
            Graphics myGraphics = Graphics.FromImage(temp);
            //源图像中要裁切的区域
            Rectangle sourceRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            ////缩小后要绘制的区域
            Rectangle destRectangle = new Rectangle(0, 0, ww, hh);
            myGraphics.Clear(Color.White);
            ////绘制缩小的图像
            myGraphics.DrawImage(bitmap, destRectangle, sourceRectangle, GraphicsUnit.Pixel);

            myGraphics.Dispose();
            return temp;
        }


        /// <summary>
        /// 获得垂直分割线
        /// 255是白，找到全是白色的线，用在分割图片
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        List<int> GetVerticalSpilterLine(Bitmap bmp)
        {
            List<int> hh = new List<int>();
            List<int> hhh = new List<int>();
            bool white;
            int w, h, a, b, r, i;
            for (w = 0; w < bmp.Width; w++)
            {
                white = true;
                for (h = 0; h < bmp.Height; h++)
                {
                    Color c = bmp.GetPixel(w, h);
                    r = Convert.ToInt32(c.R);
                    if (r == 0)
                    {
                        white = false;
                        break;
                    }
                }
                if (white)
                    hh.Add(w);
            }

            for (i = 1; i < hh.Count; i++)
            {
                a = hh[i];
                b = hh[i - 1];
                if (a - b > 3)
                {
                    hhh.Add(b);
                    hhh.Add(a);
                }
            }
            return hhh;

        }

        /// <summary>
        /// 获取水平分割线
        /// 255是白，找到全是白色的线，用在分割图片
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        List<int> GetLevelSpilterLine(Bitmap bmp)
        {

            List<int> hh = new List<int>();
            for (int h = 0; h < bmp.Height; h++)
            {
                bool white = true;
                for (int w = 0; w < bmp.Width; w++)
                {
                    Color c = bmp.GetPixel(w, h);
                    int r = Convert.ToInt32(c.R);
                    if (r == 0)
                    {
                        white = false;
                        break;
                    }
                }
                if (white)
                { //水平分割，找到Y轴的下标最大值
                    if (h < 10)
                    {
                        if (hh.Count == 0)
                            hh.Add(h);
                        else
                            hh[0] = h;
                    }
                    else
                    {//水平分割，找到Y轴的上标最小值
                        if (hh.Count == 0)
                            hh.Add(h);
                        else
                            hh.Add(h);
                        break;
                    }
                }
            }
            return hh;
        }

        /// <summary>
        /// 转换成灰度图，，行话叫二值化
        /// </summary>
        /// <param name="bp"></param>
        /// <returns></returns>
        void ConvertToGray()
        {
            for (int w = 0; w < bmp.Width; w++)
            {
                for (int h = 0; h < bmp.Height; h++)
                {
                    Color c = bmp.GetPixel(w, h);
                    int r = Convert.ToInt32(c.R);
                    if (r > 160)
                        bmp.SetPixel(w, h, Color.Black);
                    else
                        bmp.SetPixel(w, h, Color.White);
                }
            }
        }



    }
}
