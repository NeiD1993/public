using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication14
{
    public partial class Form1 : Form
    {
        private PointF old_Pos = new PointF();
        private PointF pos;
        private Rectangle curClRect;
        private int cX, cY;
        private bool det1;
        private double wh;
        public double[,] GlobalGraphContiguityMatrix;
        private double[,] GlobalGraphLengthMatrix;
        private double[][] GlobalGraphPointParameters;
        private double[][] GlobalGraphLineParameters;
        private int GlobalColumnNumber;
        private float scale = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            DoResize();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            wh = 1f;
            curClRect = pictureBox1.ClientRectangle;
        }

        private void DoResize()
        {
            wh = (GlobalGraphPointParameters[GlobalGraphPointParameters.Length - 1][0] - GlobalGraphPointParameters[0][0]) / (GlobalGraphPointParameters[GlobalGraphPointParameters.Length - 1][1] - GlobalGraphPointParameters[0][1]);
            double new_wh = pictureBox1.ClientRectangle.Width / pictureBox1.ClientRectangle.Height;
            curClRect = pictureBox1.ClientRectangle;
            if (new_wh > wh)
                curClRect.Width = (int)(curClRect.Height * wh);
            else
            {
                if (new_wh < wh)
                    curClRect.Height = (int)(curClRect.Width / wh);
                else
                    curClRect = pictureBox1.ClientRectangle;
            }
            pos.X = (pictureBox1.ClientRectangle.Width - curClRect.Width) / 2;
            pos.Y = (pictureBox1.ClientRectangle.Height - curClRect.Height) / 2;
            pictureBox1.Invalidate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        
        }

        private void Zoom(float i)
        {
            scale = scale * i;
            if (scale < 0.005f) scale = 0.005f;
            pictureBox1.Invalidate();
            pictureBox1_MouseMove(pictureBox1, new MouseEventArgs(MouseButtons.Left, 0, (int)old_Pos.X, (int)old_Pos.Y, 0));
            stBar.Items[2].Text = "" + scale;
        }

        private void ZoomIn_Click(object sender, System.EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            old_Pos = new PointF(pictureBox1.ClientRectangle.Width / 2,
                                 pictureBox1.ClientRectangle.Height / 2);
            scale = scale * 2;
            pictureBox1.Invalidate();
            stBar.Items[2].Text = "" + scale;
        }

        private void ZoomOut_Click(object sender, System.EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            old_Pos = new PointF(pictureBox1.ClientRectangle.Width / 2,
                                 pictureBox1.ClientRectangle.Height / 2);
            scale = scale / 2;
            pictureBox1.Invalidate();
            stBar.Items[2].Text = "" + scale;
        }

        private float prev_scale = 1;
        private void Shift()
        {
            pos.X = old_Pos.X - (old_Pos.X - pos.X) * scale / prev_scale;
            pos.Y = old_Pos.Y - (old_Pos.Y - pos.Y) * scale / prev_scale;
            prev_scale = scale;
        }

        private void cadPictBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            det1 = false;
            pictureBox1.Cursor = Cursors.Default;
            pictureBox1.Invalidate();
        }

        private void cadPictBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Form1.ActiveForm.Cursor = Cursors.Hand;
                cX = e.X;
                cY = e.Y;
                det1 = true;
            }
        }

        private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            pictureBox1.Focus();
            if (pictureBox1.Image == null) return;
            if (det1)
            {
                pos.X -= (cX - e.X);
                pos.Y -= (cY - e.Y);
                cX = e.X;
                cY = e.Y;
                pictureBox1.Invalidate();
            }
            old_Pos = new PointF(e.X, e.Y);
            stBar.Items[4].Text = "" + e.X + " : " + e.Y + " : 0";
        }

        private void mOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            if (openFileDialog1.FileName != null)
            {
				if(pictureBox1.Image != null)
				{	
					pictureBox1.Dispose();
					pictureBox1 = null;
				}
                this.Cursor = Cursors.WaitCursor;
                stBar.Items[0].Text = "Load file...";
                scale = 1;
                prev_scale = 1;
                pos = new PointF();
                if (openFileDialog1.FileName.ToUpper().IndexOf(".DWG") != -1)
                {
#if DWGModule 
						FCADImage = new DWG.DWGImage();
						((DWG.DWGImage)FCADImage).LoadFromFile(openFileDialog1.FileName);
						stBar.Panels[0].Text = Path.GetFileName(openFileDialog1.FileName) + " - " + DWG.DWGReader.message;
#else
                    MessageBox.Show("Not supported in current version");
                    this.Cursor = Cursors.Default;
                    pictureBox1.Visible = true;
                    return;
#endif
                }
                else 
                {
					stBar.Items[0].Text = openFileDialog1.FileName;
				}
                GetMatrices Matrix = new GetMatrices();
                Matrix.Main(openFileDialog1.FileName, out GlobalGraphContiguityMatrix, out GlobalGraphLengthMatrix, out GlobalColumnNumber, out GlobalGraphPointParameters, out GlobalGraphLineParameters);
                toolStrip1.Items[0].Enabled = true;
                toolStrip1.Items[2].Enabled = true;
                toolStrip1.Items[4].Enabled = true;
                GridFilling1(ref GlobalGraphContiguityMatrix, ref GlobalColumnNumber);
                GridFilling2(ref GlobalGraphLengthMatrix, ref GlobalColumnNumber);
                this.Cursor = Cursors.Default;
                stBar.Items[2].Text = "" + scale;
            }
        }

        private void GridFilling1(ref double[,] GlobalGraphContiguityMatrix, ref int ColumnNumber)
        {
            if (ColumnNumber == 0)
            {
                dataGrid1.Visible = false;
                goto loop7;
            }
            else
            {
                dataGrid1.Visible = true;
                dataGrid1.ColumnCount = ColumnNumber;
                dataGrid1.RowCount = ColumnNumber;
                for (int i = 0; i <= ColumnNumber - 1; i++)
                {
                    dataGrid1.Columns[i].HeaderText = Convert.ToString(i + 1);
                    dataGrid1.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
                }
                for (int j = 0; j <= ColumnNumber - 1; j++)
                {
                    for (int k = 0; k <= ColumnNumber - 1; k++)
                    {
                        dataGrid1[j, k].Value = GlobalGraphContiguityMatrix[j, k];
                    }
                }
            }
        loop7: ;
        }

        private void GridFilling2(ref double[,] GlobalGraphLengthMatrix, ref int ColumnNumber)
        {
            if (ColumnNumber == 0)
            {
                dataGrid2.Visible = false;
                goto loop8;
            }
            else
            {
                dataGrid2.Visible = true;
                dataGrid2.ColumnCount = ColumnNumber;
                dataGrid2.RowCount = ColumnNumber;
                for (int i = 0; i <= ColumnNumber - 1; i++)
                {
                    dataGrid2.Columns[i].HeaderText = Convert.ToString(i + 1);
                    dataGrid2.Rows[i].HeaderCell.Value = Convert.ToString(i + 1);
                }
                for (int j = 0; j <= ColumnNumber - 1; j++)
                {
                    for (int k = 0; k <= ColumnNumber - 1; k++)
                    {
                        dataGrid2[j, k].Value = GlobalGraphLengthMatrix[j, k];
                    }
                }
            }
        loop8: ;
        }

    }
        public class GetMatrices
        {
            public void Main(string filename, out double[,] GraphContiguityMatrixGlobal, out double[,] GraphLengthMatrixGlobal, out int ColumnNumber, out double[][] GlobalPointParameters, out double[][] GlobalLineParameters)
            {
                int count1;
                int[] count2;
                int GraphPointNumber;
                int GraphLineNumber;
                int GraphlastENTITIESindex;
                int[] GraphNotSupportedENTITIES_HT;
                int[] GraphpointersToENTITIES;
                int[] GraphpointersToENDSECS;
                string[][] AcDbFigures;
                double[][] GraphPointParameters;
                double[][] GraphLineParameters;
                int[,] GraphLineCross;
                double[,] GraphCrossPointCoordinates;
                int GraphCrossPointNumber;
                double[][] GraphPointOnLine;
                double[][] GraphEdgeLineLength;
                bool GraphDefinitionPointInLine;
                bool GraphDefinitionPointOnPoint;
                bool GraphDefinitionPointNotOnLine;
                bool GraphDefinitionPointOnCrossPoint;
                double[,] GraphContiguityMatrix;
                double[,] GraphLengthMatrix;
                int GraphPointNotOnLineNumber;
                string[] FiguresDefinition;
                GraphNotSupportedENTITIES_HT = NotSupportedENTITIES_HT(filename, out GraphpointersToENTITIES, out GraphpointersToENDSECS, out GraphlastENTITIESindex);
                if (GraphNotSupportedENTITIES_HT.Length != 0)
                {
                    double[,] a = new double[1, 1];
                    double[][] c = new double[1][];
                    c[0] = new double[1];
                    c[0][0] = 0;
                    a[0, 0] = 0;
                    GraphContiguityMatrixGlobal = a;
                    GraphLengthMatrixGlobal = a;
                    int b = 0;
                    ColumnNumber = b;
                    GlobalPointParameters = c;
                    GlobalLineParameters = c;
                    goto loop3;
                }
                else
                {
                    AcDbFigures = GetFullEntitiesArray(filename, GraphpointersToENTITIES, GraphpointersToENDSECS, GraphlastENTITIESindex, out count1, out count2);
                    FiguresDefinition = ObjectsDefinition(AcDbFigures, out GraphPointNumber, out GraphLineNumber);
                    GraphPointParameters = PointParameters(AcDbFigures, FiguresDefinition, GraphPointNumber);
                    GraphLineParameters = LineParameters(AcDbFigures, FiguresDefinition, GraphLineNumber);
                    GraphLineCross = LineCross(GraphLineParameters, out GraphCrossPointNumber);
                    GraphCrossPointCoordinates = CrossPointCoordinates(GraphLineParameters, GraphCrossPointNumber, GraphLineCross);
                    GraphPointOnLine = PointOnLine(GraphLineParameters, GraphPointParameters, out GraphPointNotOnLineNumber);
                    GraphEdgeLineLength = EdgeLineLength(GraphPointOnLine);
                    GraphDefinitionPointNotOnLine = DefinitionPointNotOnLine(GraphPointNotOnLineNumber, GraphPointParameters);
                    GraphDefinitionPointInLine = DefinitionPointInLine(GraphPointParameters, GraphLineParameters, GraphCrossPointCoordinates, GraphCrossPointNumber);
                    GraphDefinitionPointOnPoint = DefinitionPointOnPoint(GraphPointParameters);
                    GraphDefinitionPointOnCrossPoint = DefinitionPointOnCrossPoint(GraphPointOnLine, GraphCrossPointCoordinates, GraphCrossPointNumber);
                    GraphDefinition(ref GraphDefinitionPointInLine, ref GraphDefinitionPointNotOnLine, ref GraphDefinitionPointOnCrossPoint, ref GraphDefinitionPointOnPoint);
                    GraphContiguityMatrix = ContiguityMatrix(GraphEdgeLineLength, GraphPointParameters);
                    GraphLengthMatrix = LengthMatrix(GraphEdgeLineLength, GraphPointParameters);
                }
                GraphLengthMatrixGlobal = GraphLengthMatrix;
                GraphContiguityMatrixGlobal = GraphContiguityMatrix;
                ColumnNumber = GraphPointParameters.Length;
                GlobalPointParameters = GraphPointParameters;
                GlobalLineParameters = GraphLineParameters;
            loop3: ;
            }

            static int[] NotSupportedENTITIES_HT(string filename, out int[] pointersToENTITIES, out int[] pointersToENDSECS, out int lastENTITIESindex)
            {
                Hashtable ENTITIES_HT = new Hashtable();
                Hashtable ENTITIES_HTnotSupported = new Hashtable();
                Hashtable ENDSEC_HT = new Hashtable();

                string[] strings = System.IO.File.ReadAllLines(filename);

                for (int i = 0; i <= (strings.Length - 1); i++)
                {
                    if ((strings[i] == "LINE") || (strings[i] == "CIRCLE"))
                    {
                        ENTITIES_HT.Add(i, strings[i]);
                    }
                    if ((strings[i] == "LWPOLYLINE") || (strings[i] == "ARC"))
                    {
                        ENTITIES_HTnotSupported.Add(i, strings[i]);
                    }
                    if (strings[i] == "ENDSEC")
                    {
                        ENDSEC_HT.Add(i, strings[i]);
                    }
                }

                int[] pointersToENTITIESlocal = new int[ENTITIES_HT.Count];
                int k = 0;
                foreach (DictionaryEntry Item in ENTITIES_HT)
                {
                    pointersToENTITIESlocal[k] = (int)Item.Key;
                    k = k + 1;
                }

                int[] pointersToENTITIESnotSupported = new int[ENTITIES_HTnotSupported.Count];
                int p = 0;
                foreach (DictionaryEntry Item in ENTITIES_HTnotSupported)
                {
                    pointersToENTITIESnotSupported[p] = (int)Item.Key;
                    p = p + 1;
                }

                int[] pointersToENDSECSlocal = new int[ENDSEC_HT.Count];
                int l = 0;
                foreach (DictionaryEntry Item in ENDSEC_HT)
                {
                    pointersToENDSECSlocal[l] = (int)Item.Key;
                    l = l + 1;
                }

                pointersToENTITIES = pointersToENTITIESlocal;
                pointersToENDSECS = pointersToENDSECSlocal;
                lastENTITIESindex = k;

                return pointersToENTITIESnotSupported;
            }

            static string[][] GetFullEntitiesArray(string filename, int[] pointersToENTITIES, int[] pointersToENDSECS, int lastENTITIESindex, out int AcDbFiguresCount1, out int[] AcDbFiguresCount2)
            {
                string[] strings = System.IO.File.ReadAllLines(filename);

                Hashtable AcDb_HT = new Hashtable();

                bubbleSort1(ref pointersToENTITIES);
                bubbleSort1(ref pointersToENDSECS);

                int indexEndsec = 0;
                int LastEntitiesPointer = pointersToENTITIES[lastENTITIESindex - 1];

                for (int m = 0; m <= (pointersToENDSECS.Length - 1); m++)
                {
                    if (pointersToENDSECS[m] > LastEntitiesPointer)
                    {
                        indexEndsec = m;
                        break;
                    }
                }

                for (int h = 0; h <= (strings.Length - 1); h++)
                {
                    if ((strings[h] == "AcDbCircle") || (strings[h] == "AcDbLine"))
                    {
                        AcDb_HT.Add(h, strings[h]);
                    }
                }

                int[] pointersToAcDb = new int[AcDb_HT.Count];
                int a = 0;
                foreach (DictionaryEntry Item in AcDb_HT)
                {
                    pointersToAcDb[a] = (int)Item.Key;
                    a = a + 1;
                }

                bubbleSort1(ref pointersToAcDb);

                int[] sizeAcDb = new int[AcDb_HT.Count];

                int z = 0;

                while (z <= sizeAcDb.Length - 2)
                {
                    sizeAcDb[z] = pointersToENTITIES[z + 1] - pointersToAcDb[z];
                    z = z + 1;
                }

                sizeAcDb[sizeAcDb.Length - 1] = pointersToENDSECS[indexEndsec] - pointersToAcDb[a - 1];

                string[][] AcDbFigures = new string[sizeAcDb.Length][];
                int[] AcDbFiguresCountlocal = new int[sizeAcDb.Length];
                int e = 0;

                for (int g = 0; g <= sizeAcDb.Length - 1; g++)
                {
                    e = (sizeAcDb[g]) / 2 - 1;
                    AcDbFigures[g] = new string[e];
                    AcDbFiguresCountlocal[g] = e;
                }

                int increment = 2;
                int y = 2;
                int v = 0;

                for (int s = 0; s <= sizeAcDb.Length - 1; s++)
                {
                    while (y < sizeAcDb[s])
                    {
                        AcDbFigures[s][v] = strings[pointersToAcDb[s] + y];
                        y = y + increment;
                        v = v + 1;
                    }
                    y = 2;
                    v = 0;
                }

                AcDbFiguresCount1 = sizeAcDb.Length;
                AcDbFiguresCount2 = AcDbFiguresCountlocal;
                return AcDbFigures;
            }

            static void bubbleSort1(ref int[] x)
            {
                bool exchanges;
                do
                {
                    exchanges = false;
                    for (int i = 0; i < x.Length - 1; i++)
                    {
                        if (x[i] > x[i + 1])
                        {
                            int temp = x[i];
                            x[i] = x[i + 1];
                            x[i + 1] = temp;
                            exchanges = true;
                        }
                    }
                } while (exchanges);
            }

            static string[] ObjectsDefinition(string[][] Figures, out int PointNumber, out int LineNumber)
            {
                int m = 0;
                int i = 0;
                int e = 0;
                int CIRCLELength = 4;
                string[] c = new string[Figures.Length];
                for (int j = 0; j <= Figures.Length - 1; j++)
                {
                    for (int k = 0; k <= Figures[j].Length - 1; k++)
                    {
                        m = m + 1;
                    }
                    if (m == CIRCLELength)
                    {
                        c[j] = "ВЕРШИНА";
                        i = i + 1;
                    }
                    else
                    {
                        c[j] = "РЕБРО";
                        e = e + 1;
                    }
                    m = 0;
                }
                PointNumber = i;
                LineNumber = e;
                return c;
            }

            static double[][] PointParameters(string[][] Figures, string[] FiguresDefinition, int PointNumber)
            {
                int SortPositionNumber = 0;
                int PointParametersLength = 4;
                double[][] b;
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                int n = 0;
                int l = 0;
                double[][] d = new double[PointNumber][];
                for (int k = 0; k <= FiguresDefinition.Length - 1; k++)
                {
                    if (FiguresDefinition[k] == "ВЕРШИНА")
                    {
                        d[l] = new double[Figures[k].Length];
                        while (n <= Figures[k].Length - 1)
                        {
                            d[l][n] = Convert.ToDouble(Figures[k][n], provider);
                            n = n + 1;
                        }
                        n = 0;
                        l = l + 1;
                    }
                }
                PointSort(ref d, ref PointParametersLength, ref SortPositionNumber);
                b = RemoveRepeatingElements(d, PointParametersLength);
                return b;
            }

            static double[][] LineParameters(string[][] Figures, string[] FiguresDefinition, int LineNumber)
            {
                int SortPositionNumber = 0;
                int LineParametersLength = 6;
                double[][] h;
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                int i = 0;
                int f = 0;
                double[][] e = new double[LineNumber][];
                for (int k = 0; k <= FiguresDefinition.Length - 1; k++)
                {
                    if (FiguresDefinition[k] == "РЕБРО")
                    {
                        e[f] = new double[Figures[k].Length];
                        while (i <= Figures[k].Length - 1)
                        {
                            e[f][i] = Convert.ToDouble(Figures[k][i], provider);
                            i = i + 1;
                        }
                        i = 0;
                        f = f + 1;
                    }
                }
                PointSort(ref e, ref LineParametersLength, ref SortPositionNumber);
                h = RemoveRepeatingElements(e, LineParametersLength);
                return h;
            }

            static double[][] RemoveRepeatingElements(double[][] Parameters, int ParametersLength)
            {
                int b = 0;
                int h = 0;
                int k;
                int ElementNumbers = Parameters.Length;
                List<List<double>> Elements = new List<List<double>>();
                List<double> row = new List<double>();
                for (int i = 0; i <= Parameters.Length - 1; i++)
                {
                    row = new List<double>();
                    for (int j = 0; j <= ParametersLength - 1; j++) row.Add(Parameters[i][j]);
                    Elements.Add(row);
                }
                while (h < ElementNumbers)
                {
                    k = h + 1;
                    while (k < ElementNumbers)
                    {
                        switch (ParametersLength)
                        {
                            case 6:
                                {
                                    for (int q = 0; q <= ParametersLength / 2 - 1; q++)
                                    {
                                        if ((Elements[k][q] == Elements[h][q]) && (Elements[k][q + ParametersLength / 2] == Elements[h][q + ParametersLength / 2])) b += 2;
                                        else break;
                                    }
                                    if (b == ParametersLength)
                                    {
                                        Elements.RemoveAt(k);
                                        ElementNumbers -= 1;
                                        goto loop5;
                                    }
                                    b = 0;
                                    for (int y = 0; y <= ParametersLength / 2 - 1; y++)
                                    {
                                        if ((Elements[k][y] == Elements[h][y + ParametersLength / 2]) && (Elements[k][y + ParametersLength / 2] == Elements[h][y])) b += 2;
                                        else break;
                                    }
                                    if (b == ParametersLength)
                                    {
                                        Elements.RemoveAt(k);
                                        ElementNumbers -= 1;
                                    }
                                    else k += 1;
                                loop5:
                                    b = 0;
                                    break;
                                }
                            default:
                                {
                                    for (int l = 0; l <= ParametersLength - 1; l++)
                                    {
                                        if (Elements[k][l] == Elements[h][l]) b += 1;
                                        else break;
                                    }
                                    if (b == ParametersLength)
                                    {
                                        Elements.RemoveAt(k);
                                        ElementNumbers -= 1;
                                    }
                                    else k += 1;
                                    b = 0;
                                    break;
                                }
                        }
                    }
                    h += 1;
                }
                double[][] a = new double[ElementNumbers][];
                for (int n = 0; n <= a.Length - 1; n++)
                {
                    a[n] = new double[ParametersLength];
                    for (int m = 0; m <= ParametersLength - 1; m++)
                    {
                        a[n][m] = Elements[n][m];
                    }
                }
                return a;
            }

            static void PointSort(ref double[][] y, ref int ParametersNumber, ref int PositionNumber)
            {
                bool truefalse;
                int i, j, l, k;
                double u;
                for (i = 0; i <= y.Length - 2; i++)
                {
                    for (j = 0; j <= y.Length - i - 2; j++)
                    {
                        k = PositionNumber;
                        truefalse = true;
                        while ((truefalse == true) && (k <= (ParametersNumber - 1)))
                        {
                            if (y[j][k] == y[j + 1][k])
                            {
                                k = k + 1;
                            }
                            else if (y[j + 1][k] < y[j][k])
                            {
                                for (l = 0; l <= ParametersNumber - 1; l++)
                                {
                                    u = y[j][l];
                                    y[j][l] = y[j + 1][l];
                                    y[j + 1][l] = u;
                                }
                                truefalse = false;
                            }
                            else truefalse = false;
                        }
                    }
                }
            }

            static void PointSortAlternative(ref double[,] y, ref int CrossLineNumber)
            {
                int PointParametersNumber = 2;
                bool truefalse;
                int i, j, l, k;
                double u;
                for (i = 0; i <= CrossLineNumber - 2; i++)
                {
                    for (j = 0; j <= CrossLineNumber - i - 2; j++)
                    {
                        k = 0;
                        truefalse = true;
                        while ((truefalse == true) && (k <= (PointParametersNumber - 1)))
                        {
                            if (y[j, k] == y[j + 1, k])
                            {
                                k = k + 1;
                            }
                            else if (y[j + 1, k] < y[j, k])
                            {
                                for (l = 0; l <= PointParametersNumber - 1; l++)
                                {
                                    u = y[j, l];
                                    y[j, l] = y[j + 1, l];
                                    y[j + 1, l] = u;
                                }
                                truefalse = false;
                            }
                            else truefalse = false;
                        }
                    }
                }
            }

            static int[,] LineCross(double[][] LineParameters, out int CrossPointNumber)
            {
                int x = 0;
                int[,] l = new int[LineParameters.Length, LineParameters.Length];
                for (int j = 0; j <= LineParameters.Length - 2; j++)
                {
                    for (int k = j + 1; k <= LineParameters.Length - 1; k++)
                    {
                        if ((((LineParameters[k][3] - LineParameters[k][0]) * (LineParameters[j][1] - LineParameters[k][1]) - (LineParameters[k][4] - LineParameters[k][1]) * (LineParameters[j][0] - LineParameters[k][0])) * ((LineParameters[k][3] - LineParameters[k][0]) * (LineParameters[j][4] - LineParameters[k][1]) - (LineParameters[k][4] - LineParameters[k][1]) * (LineParameters[j][3] - LineParameters[k][0])) <= 0) && (((LineParameters[j][3] - LineParameters[j][0]) * (LineParameters[k][1] - LineParameters[j][1]) - (LineParameters[j][4] - LineParameters[j][1]) * (LineParameters[k][0] - LineParameters[j][0])) * ((LineParameters[j][3] - LineParameters[j][0]) * (LineParameters[k][4] - LineParameters[j][1]) - (LineParameters[j][4] - LineParameters[j][1]) * (LineParameters[k][3] - LineParameters[j][0])) <= 0))
                        {
                            l[j, k] = 1;
                            l[k, j] = 1;
                            x += 1;
                        }
                        else
                        {
                            l[j, k] = 0;
                            l[k, j] = 0;
                        }
                    }
                }
                CrossPointNumber = x;
                return l;
            }

            static double[,] CrossPointCoordinates(double[][] LineParameters, int CrossPointNumber, int[,] LineCross)
            {
                int n = 0;
                int v;
                int CrossPointCoordinatesLength = 4;
                double[,] q = new double[CrossPointNumber, CrossPointCoordinatesLength];
                for (int i = 0; i <= LineParameters.Length - 2; i++)
                {
                    for (int k = i + 1; k <= LineParameters.Length - 1; k++)
                    {
                        if ((LineCross[i, k] == 1) && (Math.Round((LineParameters[i][1] - LineParameters[i][4]) * (LineParameters[k][3] - LineParameters[k][0]) - (LineParameters[k][1] - LineParameters[k][4]) * (LineParameters[i][3] - LineParameters[i][0])) == 0))
                        {
                            v = 0;
                            for (int j = 0; j <= CrossPointCoordinatesLength - 2; j++)
                            {
                                if (LineParameters[i][j] == LineParameters[k][j]) v += 1;
                                else break;
                            }
                            if (v == CrossPointCoordinatesLength - 1)
                            {
                                q[n, 0] = LineParameters[i][0];
                                q[n, 1] = LineParameters[i][1];
                            }
                            else
                            {
                                v = 0;
                                for (int j = 0; j <= CrossPointCoordinatesLength - 2; j++)
                                {
                                    if (LineParameters[i][j + CrossPointCoordinatesLength - 1] == LineParameters[k][j + CrossPointCoordinatesLength - 1]) v += 1;
                                    else break;
                                }
                                if (v == CrossPointCoordinatesLength - 1)
                                {
                                    q[n, 0] = LineParameters[i][3];
                                    q[n, 1] = LineParameters[i][4];
                                }
                                else
                                {
                                    v = 0;
                                    for (int j = 0; j <= CrossPointCoordinatesLength - 2; j++)
                                    {
                                        if (LineParameters[i][j] == LineParameters[k][j + CrossPointCoordinatesLength - 1]) v += 1;
                                        else break;
                                    }
                                    if (v == CrossPointCoordinatesLength - 1)
                                    {
                                        q[n, 0] = LineParameters[i][0];
                                        q[n, 1] = LineParameters[i][1];
                                    }
                                    else
                                    {
                                        q[n, 0] = LineParameters[i][3];
                                        q[n, 1] = LineParameters[i][4];
                                    }
                                }
                            }

                            q[n, 2] = i;
                            q[n, 3] = k;
                            n += 1;
                        }
                        else if (LineCross[i, k] == 1)
                        {
                            q[n, 0] = (((LineParameters[k][0] * LineParameters[k][4] - LineParameters[k][3] * LineParameters[k][1]) * (LineParameters[i][3] - LineParameters[i][0]) - (LineParameters[i][0] * LineParameters[i][4] - LineParameters[i][3] * LineParameters[i][1]) * (LineParameters[k][3] - LineParameters[k][0])) / ((LineParameters[i][1] - LineParameters[i][4]) * (LineParameters[k][3] - LineParameters[k][0]) - (LineParameters[k][1] - LineParameters[k][4]) * (LineParameters[i][3] - LineParameters[i][0])));
                            q[n, 1] = (((LineParameters[k][0] * LineParameters[k][4] - LineParameters[k][3] * LineParameters[k][1]) * (LineParameters[i][4] - LineParameters[i][1]) - (LineParameters[i][0] * LineParameters[i][4] - LineParameters[i][3] * LineParameters[i][1]) * (LineParameters[k][4] - LineParameters[k][1])) / ((LineParameters[i][1] - LineParameters[i][4]) * (LineParameters[k][3] - LineParameters[k][0]) - (LineParameters[k][1] - LineParameters[k][4]) * (LineParameters[i][3] - LineParameters[i][0])));
                            q[n, 2] = i;
                            q[n, 3] = k;
                            n += 1;
                        }
                    }
                }
                PointSortAlternative(ref q, ref CrossPointNumber);
                return q;
            }

            static bool DefinitionPointOnPoint(double[][] PointParameters)
            {
                int i = 0;
                bool q = true;
                while ((i < PointParameters.Length - 1) && (q == true))
                {
                    for (int j = i + 1; j <= PointParameters.Length - 1; j++)
                    {
                        if ((PointParameters[i][0] == PointParameters[j][0]) && (PointParameters[i][1] == PointParameters[j][1]) && (PointParameters[i][2] == PointParameters[j][2]) && (PointParameters[i][3] != PointParameters[j][3]))
                        {
                            q = false;
                            break;
                        }
                    }
                    i += 1;
                }
                return q;
            }

            static double[][] PointOnLine(double[][] LineParameters, double[][] PointParameters, out int PointNotOnLineNumber)
            {
                int SortPositionNumber1 = 0;
                int SortPositionNumber2 = 5;
                int f = 0;
                int PointOnLineParametersLength = 6;
                List<List<double>> PointOnLine = new List<List<double>>();
                List<double> erow = new List<double>();
                List<int> PointOnLineAlternative = new List<int>();
                for (int d = 0; d <= LineParameters.Length - 1; d++)
                {
                    for (int e = 0; e <= PointParameters.Length - 1; e++)
                    {
                        if (Math.Round((LineParameters[d][1] - LineParameters[d][4]) * PointParameters[e][0] + (LineParameters[d][3] - LineParameters[d][0]) * PointParameters[e][1] + (LineParameters[d][0] * LineParameters[d][4] - LineParameters[d][3] * LineParameters[d][1])) == 0)
                        {
                            erow = new List<double>();
                            for (int m = 0; m <= PointParameters[e].Length - 1; m++) erow.Add(PointParameters[e][m]);
                            erow.Add(e);
                            if (PointOnLineAlternative.Contains(e) == false) PointOnLineAlternative.Add(e);
                            erow.Add(d);
                            PointOnLine.Add(erow);
                            f += 1;
                        }
                    }
                }
                int[] u = PointOnLineAlternative.ToArray();
                PointNotOnLineNumber = PointParameters.Length - u.Length;
                double[][] c = new double[f][];
                for (int k = 0; k <= f - 1; k++)
                {
                    c[k] = new double[PointOnLineParametersLength];
                    for (int h = 0; h <= PointOnLineParametersLength - 1; h++)
                    {
                        c[k][h] = PointOnLine[k][h];
                    }
                }
                PointSort(ref c, ref PointOnLineParametersLength, ref SortPositionNumber1);
                PointSort(ref c, ref PointOnLineParametersLength, ref SortPositionNumber2);
                return c;
            }

            static bool DefinitionPointNotOnLine(int PointNotOnLineNumber, double[][] PointParameters)
            {
                bool truefalse;
                if (PointNotOnLineNumber == 0) truefalse = true;
                else truefalse = false;
                return truefalse;
            }

            static bool DefinitionPointOnCrossPoint(double[][] PointOnLine, double[,] CrossPointCoordinates, int CrossPointNumber)
            {
                int b = 0;
                int CrossPointCoordinatesHalfLength = 2;
                int i = 0;
                bool exchanges = true;
                while ((i <= CrossPointNumber - 1) && (exchanges == true))
                {
                    for (int k = 0; k <= PointOnLine.Length - 1; k++)
                    {
                        b = 0;
                        for (int j = 0; j <= PointOnLine[i].Length - 5; j++)
                        {
                            if (Math.Round(PointOnLine[k][j]) == Math.Round(CrossPointCoordinates[i, j])) b += 1;
                            else break;
                        }
                        if (b == CrossPointCoordinatesHalfLength) break;
                    }
                    if (b != CrossPointCoordinatesHalfLength) exchanges = false;
                    i += 1;
                }
                return exchanges;
            }

            static double[][] EdgeLineLength(double[][] PointOnLine)
            {
                int SortPositionNumber = 1;
                int EdgeLineLengthListLength = 3;
                int b = 0;
                List<List<double>> EdgeLineLengthList = new List<List<double>>();
                List<double> row = new List<double>();
                for (int i = 0; i <= PointOnLine.Length - 2; i++)
                {
                    if (PointOnLine[i][5] == PointOnLine[i + 1][5])
                    {
                        row = new List<double>();
                        row.Add(Math.Sqrt((PointOnLine[i + 1][0] - PointOnLine[i][0]) * (PointOnLine[i + 1][0] - PointOnLine[i][0]) + (PointOnLine[i + 1][1] - PointOnLine[i][1]) * (PointOnLine[i + 1][1] - PointOnLine[i][1])));
                        row.Add(PointOnLine[i][4]);
                        row.Add(PointOnLine[i + 1][4]);
                        EdgeLineLengthList.Add(row);
                        b += 1;
                    }
                }
                double[][] a = new double[b][];
                for (int j = 0; j <= b - 1; j++)
                {
                    a[j] = new double[EdgeLineLengthListLength];
                    for (int g = 0; g <= EdgeLineLengthListLength - 1; g++)
                    {
                        a[j][g] = EdgeLineLengthList[j][g];
                    }
                }
                PointSort(ref a, ref EdgeLineLengthListLength, ref SortPositionNumber);
                double[][] z = RemoveRepeatingElements(a, EdgeLineLengthListLength);
                return z;
            }

            static bool DefinitionPointInLine(double[][] PointParameters, double[][] LineParameters, double[,] CrossPointCoordinates, int CrossPointNumber)
            {
                int b = 0;
                int j;
                int y;
                bool d = true;
                bool z = true;
                bool exchanges = false;
                int PointParametersLength = 3;
                int LineParametersLength = 6;
                double[] a = new double[LineParametersLength];
                for (int i = 0; i <= LineParameters.Length - 1; i++)
                {
                    b = 0;
                    while (b <= PointParametersLength - 1)
                    {
                        a[b] = LineParameters[i][b];
                        a[PointParametersLength + b] = LineParameters[i][PointParametersLength + b];
                        b += 1;
                    }
                    j = 0;
                    exchanges = false;
                    z = true;
                    for (int v = 0; v <= CrossPointNumber - 1; v++)
                    {
                        y = 0;
                        for (int l = 0; l <= PointParametersLength - 2; l++)
                        {
                            if (Math.Round(a[l]) == Math.Round(CrossPointCoordinates[v, l])) y += 1;
                            else break;
                        }
                        if (y == PointParametersLength - 1)
                        {
                            z = false;
                            break;
                        }
                    }
                    if (z == true)
                    {
                        while ((exchanges == false) && (j <= PointParameters.Length - 1))
                        {
                            for (int k = 0; k <= PointParametersLength - 1; k++)
                            {
                                if (a[b] == PointParameters[j][k]) b += 1;
                                else break;
                            }
                            if (b == PointParametersLength) exchanges = true;
                            else
                            {
                                j += 1;
                                b = 0;
                            }
                        }
                    }
                    else exchanges = true;
                    if (exchanges == false)
                    {
                        d = false;
                        break;
                    }
                    j = 0;
                    b = PointParametersLength;
                    exchanges = false;
                    z = true;
                    for (int q = 0; q <= CrossPointNumber - 1; q++)
                    {
                        y = 0;
                        for (int n = 0; n <= PointParametersLength - 2; n++)
                        {
                            if (Math.Round(a[n + PointParametersLength]) == Math.Round(CrossPointCoordinates[q, n])) y += 1;
                            else break;
                        }
                        if (y == PointParametersLength - 1)
                        {
                            z = false;
                            break;
                        }
                    }
                    if (z == true)
                    {
                        while ((exchanges == false) && (j <= PointParameters.Length - 1))
                        {
                            for (int k = 0; k <= PointParametersLength - 1; k++)
                            {
                                if (a[b] == PointParameters[j][k]) b += 1;
                                else break;
                            }
                            if (b == LineParametersLength) exchanges = true;
                            else
                            {
                                j += 1;
                                b = PointParametersLength;
                            }
                        }
                    }
                    else exchanges = true;
                    if (exchanges == false)
                    {
                        d = false;
                        break;
                    }
                }
                if (exchanges == true) d = true;
                return d;
            }

            static double[,] ContiguityMatrix(double[][] EdgeLineLength, double[][] PointParameters)
            {
                double[,] a = new double[PointParameters.Length, PointParameters.Length];
                for (int i = 0; i <= PointParameters.Length - 1; i++)
                {
                    for (int j = 0; j <= PointParameters.Length - 1; j++)
                    {
                        a[i, j] = 0;
                    }
                }
                for (int k = 0; k <= EdgeLineLength.Length - 1; k++)
                {
                    a[Convert.ToInt32(EdgeLineLength[k][1]), Convert.ToInt32(EdgeLineLength[k][2])] = 1;
                    a[Convert.ToInt32(EdgeLineLength[k][2]), Convert.ToInt32(EdgeLineLength[k][1])] = 1;
                }
                return a;
            }

            static double[,] LengthMatrix(double[][] EdgeLineLength, double[][] PointParameters)
            {
                double[,] b = new double[PointParameters.Length, PointParameters.Length];
                for (int i = 0; i <= PointParameters.Length - 1; i++)
                {
                    for (int j = 0; j <= PointParameters.Length - 1; j++)
                    {
                        b[i, j] = 0;
                    }
                }
                for (int k = 0; k <= EdgeLineLength.Length - 1; k++)
                {
                    b[Convert.ToInt32(EdgeLineLength[k][1]), Convert.ToInt32(EdgeLineLength[k][2])] = EdgeLineLength[k][0];
                    b[Convert.ToInt32(EdgeLineLength[k][2]), Convert.ToInt32(EdgeLineLength[k][1])] = EdgeLineLength[k][0];
                }
                return b;
            }

            static string GraphDefinition(ref bool DefinitionPointInLine, ref bool DefinitionPointOnPoint, ref bool DefinitionPointOnCrossPoint, ref bool DefinitionPointNotOnLine)
            {
                if ((DefinitionPointOnPoint == true) && (DefinitionPointNotOnLine == true) && (DefinitionPointOnCrossPoint == true) && (DefinitionPointInLine == true)) return "объект является графом";
                else return "Объект не является графом";
            }
        }
    }
