using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace VictoryWire.Shared
{
    /// <summary>
    /// Functions related to security and Cryptography
    /// </summary>
    [Serializable]
    public class Security
    {
        #region " SHA "

        /// <summary>
        /// Hash the string passed using the SHA#Cng hash. The string is transformed to a byte[] using {Strings.BytesFromString(data)}.
        /// </summary>
        /// <param name="data">The data to get a hash (signature) value.</param>
        /// <param name="hashSize">The hash size to use (1, 256, 384 or 512)</param>
        /// <returns>A Byte[] the is a unique signature of the data passed.</returns>
        /// <remarks>This function uses the Security.SHACngHash() that take a byte[] after the string passed is transformed to a byte[] .</remarks>
        public static Byte[] SHACngHash(String data, Int32 hashSize)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return Security.SHACngHash(encoding.GetBytes(data), hashSize);
        }


        /// <summary>
        /// Hash the data passed using the hash size specified defaulting to 256 if nothing is specified.
        /// </summary>
        /// <param name="data">The data to get a hash value (signature).</param>
        /// <param name="hashSize">The hash size to use (1, 256, 384 or 512)</param>
        /// <returns>A Byte[] the is a unique signature of the data passed.</returns>
        public static Byte[] SHACngHash(Byte[] data, Int32 hashSize)
        {
            Byte[] lHash = null;

            if (data != null)
            {
                if (hashSize == 512)
                    using (SHA512Cng sha = new SHA512Cng()) { lHash = sha.ComputeHash(data); }
                else if (hashSize == 384)
                    using (SHA384Cng sha = new SHA384Cng()) { lHash = sha.ComputeHash(data); }
                else if (hashSize == 256)
                    using (SHA256Cng sha = new SHA256Cng()) { lHash = sha.ComputeHash(data); }
                else
                    using (SHA1Cng sha = new SHA1Cng()) { lHash = sha.ComputeHash(data); }
            }

            return lHash;
        }

        #endregion

        #region " RSA "

        /// <summary>
        /// Perform RSA encryption of the data passed.
        /// </summary>
        /// <param name="key">RSA key.</param>
        /// <param name="data">The data to encrypt.</param>
        /// <returns></returns>
        public static String RSAEncrypt(String key, String data)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(Encoding.UTF8.GetString(Convert.FromBase64String(key)));
                byte[] b = rsa.Encrypt(Encoding.UTF8.GetBytes(data), false);
                return Convert.ToBase64String(b);
            }
        }

        /// <summary>
        /// Perform RSA decryption of the data passed.
        /// </summary>
        /// <param name="key">RSA key.</param>
        /// <param name="auth">The data to decrypt.</param>
        /// <returns></returns>
        public static String RSADecrypt(String key, String auth)
        {
            Byte[] lEncryptedBinaryToken = Convert.FromBase64String(auth);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(Encoding.UTF8.GetString(Convert.FromBase64String(key)));
                Byte[] lDecryptedBinaryToken = rsa.Decrypt(lEncryptedBinaryToken, false);
                return Encoding.UTF8.GetString(lDecryptedBinaryToken);
            }
        }

        #endregion

        #region " AES "

        /// <summary>
        /// Perform AES encryption of the data passed.
        /// </summary>
        /// <param name="key">AES key</param>
        /// <param name="initializationVector">AES initialization vector</param>
        /// <param name="data">The binary data to encrypt.</param>
        /// <returns>Encrypted <c>Byte[]</c></returns>
        public static Byte[] AESEncrypt(Byte[] key, Byte[] initializationVector, Byte[] data)
        {
            byte[] lEncrypted = null; /*Holds the encrypted data*/

            using (AesCryptoServiceProvider lAESAlg = new AesCryptoServiceProvider())
            {
                using (ICryptoTransform lCryptor = lAESAlg.CreateEncryptor(key, initializationVector))
                {
                    lEncrypted = Security.AESCrypter(lCryptor, data);
                }
            }

            return lEncrypted; /*Return the encrypted binary data*/
        }

        /// <summary>
        /// Perform AES decryption of the encrypted data passed.
        /// </summary>
        /// <param name="key">AES key</param>
        /// <param name="initializationVector">AES initialization vector</param>
        /// <param name="encryptedData">The binary data to decrypt.</param>
        /// <returns>Decrypted <c>Byte[]</returns>
        public static Byte[] AESDecrypt(Byte[] key, Byte[] initializationVector, Byte[] encryptedData)
        {
            byte[] lDecrypted = null; /*Holds the decrypted data*/

            using (AesCryptoServiceProvider lAESAlg = new AesCryptoServiceProvider())
            {
                using (ICryptoTransform lCryptor = lAESAlg.CreateDecryptor(key, initializationVector))
                {
                    lDecrypted = Security.AESCrypter(lCryptor, encryptedData);
                }
            }

            return lDecrypted; /*Return the decrypted binary data*/
        }

        /// <summary>
        /// Perform AES cryptography of the data passed.
        /// </summary>
        /// <param name="cryptor">The crypto transform to use against the data passed.</param>
        /// <param name="data">Source data to perform cryptography.</param>
        /// <returns>Byte[]</returns>
        private static Byte[] AESCrypter(ICryptoTransform cryptor, Byte[] data)
        {
            /*Holds the encrypted/decrypted byte[]*/
            byte[] lCrypted = null;

            using (MemoryStream lMemoryStream = new MemoryStream())
            {
                using (CryptoStream lCryptoStream = new CryptoStream(lMemoryStream, cryptor, CryptoStreamMode.Write))
                {
                    //Do the actual encryption/decryption of the data passed
                    lCryptoStream.Write(data, 0, data.Length);
                    lCryptoStream.FlushFinalBlock();
                    lCrypted = lMemoryStream.ToArray();
                }
            }

            /*Return the encrypted/decrypted byte[]*/
            return lCrypted;
        }
        #endregion

        #region " XML Methods "

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rsa"></param>
        /// <returns></returns>
        public static Boolean VerifyXmlDocumentEnvelopedSignature(XmlDocument xml, RSACryptoServiceProvider rsa)
        {
            Boolean lSigned = false;

            if (xml != null && rsa != null)
            {
                //This does the signature verification
                SignedXml lSignedXml = new SignedXml(xml);

                //Since this is an enveloped signature, the signature is contained in the XML passed
                XmlNodeList lSignature = xml.GetElementsByTagName("Signature");

                if (lSignature == null || lSignature.Count != 1)
                {
                    //No signature was found, so the XML passed cannot possibly be signed
                }
                else
                {
                    //Load the signature into the signing object and check the signature
                    lSignedXml.LoadXml((XmlElement)lSignature[0]);
                    lSigned = lSignedXml.CheckSignature(rsa);
                }
            }
            else
            {
                //The XML or RSA information was null, signature verification is impossible
            }

            return lSigned;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rsa"></param>
        /// <returns></returns>
        public static XmlDocument SignXmlDocumentEnveloped(XmlDocument xml, RSACryptoServiceProvider rsa)
        {
            XmlDocument lSignedXml = null;

            if (xml != null && rsa != null)
            {
                //Make a copy of the source XML to be returned as the signed XML
                lSignedXml = new XmlDocument();
                lSignedXml.LoadXml(xml.DocumentElement.OuterXml);

                //This does the signing 
                SignedXml lSigner = new SignedXml(lSignedXml) { SigningKey = rsa };

                //Allows for setting what parts of the XML should be signed
                Reference reference = new Reference();
                reference.Uri = "";

                //Add an enveloped transformation to the reference
                //This signature method accounts for placing the signature INSIDE the body of the XML being signed
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

                //Add the reference to the SignedXml object
                lSigner.AddReference(reference);

                //Compute the signature
                lSigner.ComputeSignature();

                //Get the XML representation of the signature and return it
                XmlElement xmlDigitalSignature = lSigner.GetXml();

                //Append the signature to the XML document
                lSignedXml.DocumentElement.AppendChild(xmlDigitalSignature);
            }
            else
            {
                //The XML or RSA information was null, signature verification is impossible
                //<robot voice> "cannot compute"
            }

            return lSignedXml;
        }

        #endregion

        #region " XElement Methods "

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rsa"></param>
        /// <returns></returns>
        public static Boolean VerifyXElementEnvelopedSignature(XElement xml, RSACryptoServiceProvider rsa)
        {
            //We need an XML document to do the signing
            XmlDocument lXml = new XmlDocument();
            lXml.LoadXml(xml.ToString());

            //Update the XElement passed with the signature
            Boolean lSigned = Security.VerifyXmlDocumentEnvelopedSignature(lXml, rsa);

            return lSigned;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="rsa"></param>
        /// <returns></returns>
        public static XElement SignXElementEnveloped(XElement xml, RSACryptoServiceProvider rsa)
        {
            //We need an XML document to do the signing
            XmlDocument lXml = new XmlDocument();
            lXml.LoadXml(xml.ToString());

            //Update the XElement passed with the signature
            XmlDocument lSignedXml = Security.SignXmlDocumentEnveloped(lXml, rsa);
            if (lSignedXml != null)
            {
                //Signing worked
                return XElement.Parse(lSignedXml.DocumentElement.OuterXml);
            }
            else
            {
                //Signing failed
                return null;
            }
        }

        #endregion

        #region " Helper Methods "

        /// <summary>
        /// Return a hex string representation of a byte array.
        /// </summary>
        /// <param name="bytes">Bytes to convert to hex</param>
        /// <param name="removeDashes">This function uses {BitConverter} to create a byte string which separates bytes by dashes, send {true} to remove them.</param>
        /// <returns>A string representation of a byte[] (23-3A-F3-82-CB-23-E9 or 233AF382CB23E9 when removedDashes is {true})</returns>
        public static String ByteArrayToHex(Byte[] bytes, Boolean removeDashes)
        {
            String lHex = String.Empty;

            if (!(bytes == null || (Object)bytes == DBNull.Value) && bytes.Length > 0)
            {
                lHex = BitConverter.ToString(bytes);
                if (removeDashes) { lHex = lHex.Replace("-", String.Empty); }
            }

            return lHex;
        }

        #endregion

    }
}
