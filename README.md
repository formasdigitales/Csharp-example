# Csharp-example
Ejemplo de Timbrado con C#

Este ejemplo nos muestra como timbrar con c# a nuestro webservice de pruebas.

Contamos con las clase _ClienteFormasDigitales_ que es la que nos ayuda a agregar los datos del numero de certificado, el sello y el certificado en el cfdi que enviaremos al servicio web para poder timbrarlo.

Esta clase se inicializa enviandole el path del xml:

```C#
ClienteFormasDigitales cf = new ClienteFormasDigitales("xmlpath");
``` 

En el ejemplo tenemos la interfaz grafica para seleccionar el archivo xml que deseamos enviar a timbrar. 

En la clase _ClienteFormasDigitales_ tenemos el metodo _SellarXML_ que recibe de parametros el path del certificado y de la key y nos devuelve el xml con el sello, el numero de certificado y el certificado en base64, tambien tiene el metodo _GetXmlString_ al cual le pasamos un _XmlDocument_ para que nos regrese nuestro xml en string para posteriormente enviarlo a nuestro servicio web.

```C#
ClienteFormasDigitales cf = new ClienteFormasDigitales("xmlpath");
XmlDocument xmlSellado = cf.SellarXML(txtCertificado.Text, keyPath: txtKey.Text);
string xmlString = cf.GetXmlString(xmlSellado);
``` 

Las Clases _TimbrarCFDIRequest_ , _WSTimbradoCFDIClient_ y _TimbrarCFDIResponse_ se generan automaticamente con la herramienta de visual studio, en el explorador de soluciones, en connected Services agregamos un nuevo servicio y pegamos la url del wsdl para generar las clases correspondientes.

