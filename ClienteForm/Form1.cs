using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClienteForm.FormasServices;

namespace ClienteForm
{
    
    public partial class Form1 : Form
    {
        
        private WaitForm wf;
        public Form1()
        {
            InitializeComponent();
        }

       private void ProcessData(object sender, DoWorkEventArgs e)
        {
            ClienteFormasDigitales cf = new ClienteFormasDigitales(txtXML.Text);

            String xmlString = cf.GetXmlString(cf.SellarXML(txtCertificado.Text, txtKey.Text));

            TimbrarCFDIRequest tcr = new TimbrarCFDIRequest();

            accesos acc = new accesos();
            acc.password = "pruebasWS";
            acc.usuario = "pruebasWS";
            tcr.comprobante = xmlString;
            tcr.accesos = acc;

            WSTimbradoCFDI ws = new WSTimbradoCFDIClient();

            TimbrarCFDIResponse response = ws.TimbrarCFDI(tcr);

            if (response.acuseCFDI.error != null)
            {
               e.Result = response.acuseCFDI.error;
            }
            else
            {
                e.Result = response.acuseCFDI.xmlTimbrado;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += new DoWorkEventHandler(ProcessData);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(completeProcess);

            // Start the worker.
            bg.RunWorkerAsync();
            wf = new WaitForm();
            wf.ShowDialog();
        }

        private void completeProcess(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                richTextBox1.Text += e.Result as string;
            }
            wf.Close();
        }
    
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "'XML files|*.xml";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            if(ofd.ShowDialog() == DialogResult.OK) {
                txtXML.Text = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "'Certificates files|*.cer";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtCertificado.Text = ofd.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "'Key files|*.key";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtKey.Text = ofd.FileName;
            }
        }
    }
}
