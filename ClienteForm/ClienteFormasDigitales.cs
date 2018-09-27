using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Reflection;
using JavaScience;

namespace ClienteForm {
    class ClienteFormasDigitales{
        private XmlDocument xml;
        private string FORMATO_FECHA = "yyyy-MM-dd HH:mm:ss";

        public ClienteFormasDigitales(string xmlPath)
        {
            this.xml = new XmlDocument();
                try
            {
                xml.Load(xmlPath);
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public string GetXmlString(XmlDocument xml)
        {
           
            StringWriter sw  = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            xml.WriteTo(xw);
            return sw.ToString();
        }

     
        public XmlDocument SellarXML(string certPath, string keyPath)
        {

            X509Certificate2 certificate = new X509Certificate2(certPath);
          
            //RSACryptoServiceProvider rsa=OpenKeyFile(filename, pPassword);
            SHA256 sha256 = SHA256.Create();
            UTF8Encoding encoding = new UTF8Encoding();
            string fecha = DateTime.Now.ToString(FORMATO_FECHA);
            this.xml.DocumentElement.SetAttribute("Fecha", fecha.Replace(" ", "T"));
            string certificado = GetCertificado(certificate);
            string noCertificado = GetNoCertificado(certificate);
            this.xml.DocumentElement.SetAttribute("NoCertificado", noCertificado);

            string sello = GetSello(keyPath, this.xml);
            this.xml.DocumentElement.SetAttribute("Certificado", certificado);
            this.xml.DocumentElement.SetAttribute("Sello", sello);

            return this.xml;

        } 

        private string GetSello(string keyPath, XmlDocument document)
        {
             System.Security.SecureString passwordSeguro = new System.Security.SecureString();
             string passKey = "12345678a";
             passwordSeguro.Clear();
             foreach (char c in passKey.ToCharArray())
                passwordSeguro.AppendChar(c);

            byte[] keyBytes = System.IO.File.ReadAllBytes(keyPath);
            RSACryptoServiceProvider rsa = opensslkey.DecodeEncryptedPrivateKeyInfo(keyBytes, passwordSeguro);
            SHA256 sha256 = SHA256.Create();
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] cadenaBytes = encoding.GetBytes(GetCadenaOriginal(this.xml));
            byte[] digest = sha256.ComputeHash(cadenaBytes);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa);
            RSAFormatter.SetHashAlgorithm("SHA256");
            byte[] SignedHashValue = RSAFormatter.CreateSignature(digest);

            string sello = Convert.ToBase64String(SignedHashValue);

            return sello;
        }

        private string GetCadenaOriginal(XmlDocument document)
        {
            XmlDocument xsltDoc = new XmlDocument();

            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                StreamReader reader = new StreamReader(asm.GetManifestResourceStream("ClienteForm.Files.cadenaoriginal_3_3.xslt"));
                xsltDoc.Load(reader);
            }
            catch(XmlException e)
            {
                Console.WriteLine(e.Message);
            }

            
            XsltSettings sets = new XsltSettings(true, true);
            var resolver = new XmlUrlResolver();
            XslCompiledTransform myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(xsltDoc,sets, resolver);
            
            StringWriter sw = new StringWriter();
            myXslTrans.Transform(document,null,sw);
            return sw.ToString();           
        }
        private string GetNoCertificado(X509Certificate2 certificate)
        {
            string serialNumber = certificate.GetSerialNumberString();
            string noCertificado = "";
            int len = serialNumber.Length;

            if((len % 2) == 1)
            {
                serialNumber = serialNumber + " ";
                len++;
            }

            for (int i = 0; i < (len / 2); i++)
            {
                string aux = serialNumber.Substring(i * 2,  2);
                noCertificado = noCertificado + (aux.Substring(1));
            }

            char[] charArray = noCertificado.ToCharArray();
            return new string( charArray );

        }

        private string GetCertificado(X509Certificate2 certificate)
        {
            StringBuilder builder = new StringBuilder(); 
            builder.AppendLine(Convert.ToBase64String(certificate.Export(X509ContentType.Cert)));
            String certificado = builder.ToString();
            certificado  = certificado.Replace("\r\n", string.Empty);
            return certificado  ;
        }

    }
}