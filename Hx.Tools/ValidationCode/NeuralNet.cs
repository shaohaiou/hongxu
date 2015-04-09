using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Tools.ValidationCode
{
    public class NeuralNet
    {//相关系数
        const double momentum = 0;
        //最小均方误差
        const double minex = 0.001;
        //BP网络隐层结点的数目
        const int hidden = 10;
        //训练步长
        const double eta = 0.015;
        //网络输出层结点的个数
        const int output = 4;
        //输入层结点个数,,待识别图片的像素点个数
        const int input = 156;


        //理想输出模板
        static double[] output0 = { 0.1, 0.1, 0.1, 0.1 };
        static double[] output1 = { 0.1, 0.1, 0.1, 0.9 };
        static double[] output2 = { 0.1, 0.1, 0.9, 0.1 };
        static double[] output3 = { 0.1, 0.1, 0.9, 0.9 };
        static double[] output4 = { 0.1, 0.9, 0.1, 0.1 };
        static double[] output5 = { 0.1, 0.9, 0.1, 0.9 };
        static double[] output6 = { 0.1, 0.9, 0.9, 0.1 };
        static double[] output7 = { 0.1, 0.9, 0.9, 0.9 };
        static double[] output8 = { 0.9, 0.1, 0.1, 0.1 };
        static double[] output9 = { 0.9, 0.1, 0.1, 0.9 };

        static double[][] outnumber = { output0, output1, output2, output3, output4, output5, output6, output7, output8, output9 };

        //指向输入层于隐层之间权值的指针
        static double[][] input_weights;
        //指向隐层与输出层之间的权值的指针
        static double[][] hidden_weights;
        //指向上一此输入层于隐层之间权值的指针
        static double[][] input_prev_weights;
        //指向上一此隐层与输出层之间的权值的指针
        static double[][] hidden_prev_weights;

        static NeuralNet()
        {
            ran = new Random();
            input_weights = new double[input + 1][];
            hidden_weights = new double[hidden + 1][];
            input_prev_weights = new double[input + 1][];
            hidden_prev_weights = new double[hidden + 1][];

            //对各种权值进行初始化            
            randomize_weights(input_weights, input, hidden);
            randomize_weights(hidden_weights, hidden, output);
            zero_weights(input_prev_weights, input, hidden);
            zero_weights(hidden_prev_weights, hidden, output);
        }



        static double squash(double x)
        {
            return (1.0 / (1.0 + Math.Exp(-x)));
        }

        //生成-1~1的随机数
        static Random ran;
        static double getrandom()
        {
            double a = ran.NextDouble();
            double b = ran.NextDouble();
            return a - b;
        }

        /// <summary>
        /// 随机初始化权值
        /// </summary>
        /// <param name="w"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        static void randomize_weights(double[][] w, int m, int n)
        {
            for (int i = 0; i <= m; i++)
            {
                double[] temp = new double[n + 1];
                for (int j = 0; j <= n; j++)
                    temp[j] = getrandom();
                w[i] = temp;
            }
        }
        /// <summary>
        /// 0初始化权值
        /// </summary>
        /// <param name="?"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        static void zero_weights(double[][] w, int m, int n)
        {
            for (int i = 0; i <= m; i++)
            {
                double[] temp = new double[n + 1];
                for (int j = 0; j <= n; j++)
                    temp[j] = 0.0;
                w[i] = temp;
            }
        }

        /// <summary>
        /// 前向传输
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="conn"></param>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        static void LayerForward(double[] curr, double[] next, double[][] weights, int n1, int n2)
        {
            double sum;
            /*** 设置偏置对应输入层值 ***/
            curr[0] = 1.0;
            /*** 对于第二层的每个神经元 ***/
            for (int j = 1; j <= n2; j++)
            {
                /*** 计算输入的加权总和 ***/
                sum = 0.0;
                for (int k = 0; k <= n1; k++)
                    sum += weights[k][j] * curr[k];
                next[j] = squash(sum);
            }
        }

        /// <summary>
        /// 输出误差
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="target"></param>
        /// <param name="output"></param>
        /// <param name="nj"></param>
        static void OutputError(double[] delta, double[] target, double[] output, int nj)
        {
            double o, t;
            for (int j = 1; j <= nj; j++)
            {
                o = output[j];
                t = target[j];
                delta[j] = o * (1.0 - o) * (t - o);
            }
        }

        /// <summary>
        /// 隐含层误差
        /// </summary>
        /// <param name="delta_h"></param>
        /// <param name="nh"></param>
        /// <param name="delta_o"></param>
        /// <param name="no"></param>
        /// <param name="who"></param>
        /// <param name="hidden"></param>
        static void HiddenError(double[] delta_h, int nh, double[] delta_o, int no, double[][] weights, double[] hidden)
        {
            double h, sum;
            for (int j = 1; j <= nh; j++)
            {
                h = hidden[j];
                sum = 0.0;
                for (int k = 1; k <= no; k++)
                    sum += delta_o[k] * weights[j][k];
                delta_h[j] = h * (1.0 - h) * sum;
            }
        }



        /* 调整权值 */
        static void AdjustWeights(double[] delta, int ndelta, double[] ly, int nly, double[][] w, double[][] oldw, double eta, double momentum)
        {
            double new_dw;
            ly[0] = 1.0;
            for (int j = 1; j <= ndelta; j++)
            {
                for (int k = 0; k <= nly; k++)
                {
                    new_dw = ((eta * delta[j] * ly[k]) + (momentum * oldw[k][j]));
                    w[k][j] += new_dw;
                    oldw[k][j] = new_dw;
                }
            }
        }

        /// <summary>
        /// 根据输入的特征向量和期望的理想输出向量对BP网络尽行训练,训练结束后将权值保存
        /// </summary>
        /// <param name="data_in">输入的特征向量</param>
        /// <param name="data_out">理想输出特征向量</param>
        public static bool Train(List<List<double>> data_in, List<int> data_out)
        {
            //循环变量
            int i, l, k, flag;
            //指向输入层数据的指针
            double[] input_layer = new double[input + 1];
            //指向隐层数据的指针
            double[] hidden_layer = new double[input + 1];
            //指向输出层数据的指针
            double[] output_layer = new double[hidden + 1];
            //指向隐层误差数据的指针
            double[] hidden_deltas = new double[hidden + 1];
            //指向输出层误差数剧的指针
            double[] output_deltas = new double[output + 1];
            //指向理想目标输出的指针
            double[] target = new double[output + 1];
            //每次循环后的均方误差误差值 
            double ex = double.MaxValue;

            //开始进行BP网络训练
            //这里设定最大的迭代次数为15000次
            for (l = 0; l < 15000; l++)
            {
                //对均方误差置零
                ex = 0;
                for (k = 0; k < data_in.Count; k++)
                {
                    //将提取的样本的特征向量输送到输入层上
                    for (i = 1; i <= input; i++)
                        input_layer[i] = data_in[k][i - 1];

                    flag = data_out[k];
                    //将预定的理想输出输送到BP网络的理想输出单元
                    for (i = 1; i <= output; i++)
                        target[i] = outnumber[flag][i - 1];

                    //前向传输激活
                    //将数据由输入层传到隐层 
                    LayerForward(input_layer, hidden_layer, input_weights, input, hidden);
                    //将隐层的输出传到输出层
                    LayerForward(hidden_layer, output_layer, hidden_weights, hidden, output);
                    //误差计算
                    //将输出层的输出与理想输出比较计算输出层每个结点上的误差
                    OutputError(output_deltas, target, output_layer, output);
                    //根据输出层结点上的误差计算隐层每个节点上的误差
                    HiddenError(hidden_deltas, hidden, output_deltas, output, hidden_weights, hidden_layer);
                    //权值调整
                    //根据输出层每个节点上的误差来调整隐层与输出层之间的权值
                    AdjustWeights(output_deltas, output, hidden_layer, hidden, hidden_weights, hidden_prev_weights, eta, momentum);
                    //根据隐层每个节点上的误差来调整隐层与输入层之间的权值    	
                    AdjustWeights(hidden_deltas, hidden, input_layer, input, input_weights, input_prev_weights, eta, momentum);
                    //误差统计
                    for (i = 1; i <= output; i++)
                        ex = (output_layer[i] - outnumber[flag][i - 1]) * (output_layer[i] - outnumber[flag][i - 1]);
                }
                ex = ex / Convert.ToDouble(data_in.Count * output);
                if (ex < minex) break;
            }

            if (ex <= minex)
                return true;
            else
                return false;
        }

        ///<summary>
        ///读入输入样本的特征相量并根据训练所得的权值,进行识别
        ///</summary>
        ///<param name="data_in"></param>
        public static int Recognize(List<double> data_in)
        {
            int i, result;

            result = 0;
            //指向输入层数据的指针
            double[] input_layer = new double[input + 1];
            //指向隐层数据的指针
            double[] hidden_layer = new double[input + 1];
            //指向输出层数据的指针
            double[] output_layer = new double[hidden + 1];


            //将提取的样本的特征向量输送到输入层上
            for (i = 1; i <= input; i++)
                input_layer[i] = data_in[i - 1];

            //前向输入激活
            LayerForward(input_layer, hidden_layer, input_weights, input, hidden);
            LayerForward(hidden_layer, output_layer, hidden_weights, hidden, output);

            //考察每一位的输出
            //如果大于0.5判为1
            for (i = 1; i <= output; i++)
            {
                if (output_layer[i] > 0.5)
                    result += (int)Math.Pow(2, Convert.ToDouble(4 - i));
            }
            return result;
        }

    }
}