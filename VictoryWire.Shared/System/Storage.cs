using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace VictoryWire.Shared
{
    /// <summary>
    /// Manages blob access across local/azure blob storage.
    /// </summary>
    public class Storage
    {

        #region " Members "

        private CloudBlobClient BlobClient { get; set; }        

        private String BlobConnection { get; set; }

        private String BlobContainer { get; set; }

        #endregion

        #region " Constructors "

        public Storage()
        {
            this.BlobConnection = WebConfigurationManager.AppSettings["BlobConnection"].ToString();
            this.BlobContainer = WebConfigurationManager.AppSettings["BlobContainer"].ToString();

            CloudStorageAccount lCloudAccount = CloudStorageAccount.Parse(this.BlobConnection);
            this.BlobClient = lCloudAccount.CreateCloudBlobClient();
        }


        #endregion

        #region " Read / Write "

        /// <summary>
        /// Reads blob into byte array.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public Byte[] ReadAllBytes(String container, String blob)
        {
            Byte[] lBytes = null;

            using (MemoryStream ms = this.OpenRead(container, blob))
            {
                if (ms.Length > 0)
                {
                    lBytes = ms.ToArray();
                }
            }

            return lBytes;
        }

        /// <summary>
        /// Returns a memory stream of the blob.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public MemoryStream OpenRead(string container, string blob)
        {
            MemoryStream lStream = new MemoryStream();

            CloudBlockBlob lBlob = this.GetBlob(container, blob);
            if (lBlob.Exists())
            {
                //blob exists 
                lBlob.DownloadToStream(lStream);
            }

            //reset
            lStream.Position = 0;

            return lStream;
        }
        /// <summary>
        /// Writes a byte array to a blob.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <param name="content"></param>
        public Uri WriteAllBytes(String blob, Byte[] content, String mimeType)
        {
            CloudBlockBlob lBlob = this.GetBlob(this.BlobContainer, blob);
            if (!lBlob.Exists())
            {
                //Create a brand new blob
                lBlob.UploadFromByteArray(content, 0, content.Length);
                lBlob.Properties.ContentType = mimeType;
                lBlob.SetProperties();
            }
            else
            {
                //Overwrite an existing blob
                lBlob.UploadFromByteArray(content, 0, content.Length);
                lBlob.Properties.ContentType = mimeType;
                lBlob.SetProperties();
            }
            return lBlob.Uri;

        }

        #endregion

        #region " Blob/Container "

        /// <summary>
        /// Gets a blob from azure blob storage.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        private CloudBlockBlob GetBlob(String container, String blob)
        {
            CloudBlobContainer lContainer = this.GetContainer(container);
            CloudBlockBlob lBlob = lContainer.GetBlockBlobReference(blob);
            return lBlob;
        }

        /// <summary>
        /// Gets a reference to a container from azure blob storage.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        private CloudBlobContainer GetContainer(String container, Boolean create = false)
        {
            CloudBlobContainer lContainer = this.BlobClient.GetContainerReference(container);
            if (create)
            {
                lContainer.CreateIfNotExists();
                lContainer.Create(BlobContainerPublicAccessType.Blob);
            }

            return lContainer;
        }

        #endregion

        #region " Helper Methods "

        /// <summary>
        /// Container/folder exists.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public Boolean ContainerExists(string container)
        {
            return this.GetContainer(container).Exists();
        }

        /// <summary>
        /// Blob/folder exists.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        /// <returns></returns>
        public Boolean BlobExists(string container, string blob)
        {

            return this.GetBlob(container, blob).Exists();

        }

        /// <summary>
        /// Delete blobs.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="blob"></param>
        public void DeleteBlob(string container, string blob)
        {

            CloudBlockBlob lBlob = this.GetBlob(container, blob);
            if (lBlob.Exists())
            {
                //blob exists 
                lBlob.Delete();
            }

        }

        #endregion

    }

}