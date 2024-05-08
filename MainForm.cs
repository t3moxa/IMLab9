namespace IMLab83
{
    public partial class MainForm : Form
    {
        int[] _statistics = new int[6];
        double[] _empyricalProbability = new double[6];
        double[] _realProbability = new double[6];
        int N = 0;
        double _empyricalVariance = 0;
        double _empyricalMean = 0;
        double _realVariance = 0;
        double _realMean = 0;
        double _criteriaChi = 11.070; //m = 5, a = 0,05
        double _realChi = 0;
        double _meanError = 0;
        double _varianceError = 0;
        Random _rnd = new Random();
        public MainForm()
        {
            InitializeComponent();
            for (int i = 0; i < 6; i++)
            {
                _statistics[i] = 0;
            }
        }
        void NullEverything()
        {
            for (int i = 0; i < 6; i++)
            {
                _statistics[i] = 0;
                _empyricalProbability[i] = 0;
                _realProbability[i] = 0;
            }
            _empyricalMean = 0;
            _empyricalVariance = 0;
            _realMean = 0;
            _realVariance = 0;
            _criteriaChi = 0;
            _realChi = 0;
            _meanError = 0;
            _varianceError = 0;
        }
        void CalculateEmpyricals()
        {
            for (int i = 0; i < 6; i++)
            {
                _empyricalMean += _empyricalProbability[i] * (i+1);
            }
            for (int i = 0; i < 6; i++)
            {
                _empyricalVariance += _empyricalProbability[i]*Math.Pow((i+1)-_empyricalMean,2);
            }
        }
        void CalculateReal()
        {
            for (int i = 0; i < 6; i++)
            {
                _realProbability[i] = (double)_statistics[i] / (double)N;
                _realMean += _realProbability[i] * (i + 1);
            }
            for (int i = 0; i < 6; i++)
            {
                _realVariance += _realProbability[i] * Math.Pow((i + 1) - _realMean, 2);
            }
            _meanError = Math.Abs(_realMean - _empyricalMean)/Math.Abs(_realMean);
            _varianceError = Math.Abs(_realVariance - _empyricalVariance)/Math.Abs(_realVariance);
            for (int i = 0; i < 6;i++)
            {
                _realChi += Math.Pow(_statistics[i], 2) / (_empyricalProbability[i] * N);
            }
            _realChi -= N;
        }
        void WriteParameters()
        {
            MeanLabel.Text = "Выборочное среднее: " + Math.Round(_realMean,2).ToString() + " Погрешность: " + Math.Round(_meanError*100,2).ToString() + "%";
            VarianceLabel.Text = "Дисперсия: " + Math.Round(_realVariance,2).ToString() + " Погрешность: " + Math.Round(_varianceError*100,2).ToString() + "%";
            if (_realChi>_criteriaChi)
            {
                ChiLabel.Text = "Критерий хи-квадрат: " + Math.Round(_realChi,2).ToString() + " > " + _criteriaChi.ToString() + " FALSE";
            }
            else
            {
                ChiLabel.Text = "Критерий хи-квадрат: " + Math.Round(_realChi, 2).ToString() + " < " + _criteriaChi.ToString() + " TRUE";
            }

        }
        double CalculateChi(int N)
        {
            double chi = 0;
            for (int i = 0; i < 6; i++)
            {
                chi += Math.Pow(_statistics[i], 2) / (_empyricalProbability[i] * N);
            }
            chi -= N;
            return chi;
        }
        void AssignProbabilities()
        {
            _empyricalProbability[0] = Double.Parse(ProbBox1.Text);
            _empyricalProbability[1] = Double.Parse(ProbBox2.Text);
            _empyricalProbability[2] = Double.Parse(ProbBox3.Text);
            _empyricalProbability[3] = Double.Parse(ProbBox4.Text);
            _empyricalProbability[4] = Double.Parse(ProbBox5.Text);
        }
        void Calculate6Probability()
        {
            _empyricalProbability[5] = 1 - _empyricalProbability[0] - _empyricalProbability[1] - _empyricalProbability[2] - _empyricalProbability[3] - _empyricalProbability[4];
            ProbBox6.Text = _empyricalProbability[5].ToString();
        }
        int GenerateEvent()
        {
            double a = _rnd.NextDouble();
            int k = 0;
            while (a > 0)
            {
                a -= _empyricalProbability[k];
                k++;
            }
            return k - 1;
        }
        void DrawChart()
        {
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < 6; i++)
            {
                chart1.Series[0].Points.AddXY(i, _statistics[i]);
            }
        }
        Label CreateLabel()
        {
            Label label = new Label();
            label.TextAlign = ContentAlignment.MiddleCenter;
            
            label.Dock = DockStyle.Fill;
            label.Width = 150;
            return label;
        }
        void FillTable()
        {
            for (int i = 1; i <= 6; i++)
            {
                tableLayoutPanel1.Controls.Add(CreateLabel(), i, 0);
                tableLayoutPanel1.GetControlFromPosition(i, 0).Text = i.ToString();
            }
            tableLayoutPanel1.Controls.Add(CreateLabel(), 7, 0);
            tableLayoutPanel1.GetControlFromPosition(7, 0).Text = "Хи";
            for (int i = 1; i < 5; i++)
            {
                tableLayoutPanel1.Controls.Add(CreateLabel(), 0, i);
                tableLayoutPanel1.GetControlFromPosition(0, i).Text = Math.Pow(10,i).ToString();
            }
        }
        void GenerateSelection(int amount)
        {
            AssignProbabilities();
            Calculate6Probability();
            for (int i = 0; i < amount; i++)
            {
                _statistics[GenerateEvent()]++;
            }
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_empyricalProbability[0] + _empyricalProbability[1] + _empyricalProbability[2] + _empyricalProbability[3] + _empyricalProbability[4] < 1)
            {
                NullEverything();
                AssignProbabilities();
                Calculate6Probability();
                N = Int32.Parse(NumberOfTrialsBox.Text);
                for (int i = 0; i < N; i++)
                {
                    _statistics[GenerateEvent()]++;
                }
                DrawChart();
                CalculateEmpyricals();
                CalculateReal();
                WriteParameters();
            }
        }
        void WriteStatistics(int row, int N)
        {
            for (int i = 1; i <= 6; i++)
            {
                tableLayoutPanel1.Controls.Add(CreateLabel(), i, row);
                tableLayoutPanel1.GetControlFromPosition(i, row).Text = _statistics[i-1].ToString();
            }
            tableLayoutPanel1.Controls.Add(CreateLabel(), 7, row);
            tableLayoutPanel1.GetControlFromPosition(7, row).Text = Math.Round(CalculateChi(N),2).ToString();
        }
        private void FillTableButton_Click(object sender, EventArgs e)
        {
            FillTable();
            int N;
            for (int i = 1; i < 5; i++)
            {
                N = (int)Math.Pow(10, i);
                GenerateSelection(N);
                WriteStatistics(i, N);
                for (int j = 0; j < 6; j++)
                {
                    _statistics[j] = 0;
                }
            }
        }
    }
}
