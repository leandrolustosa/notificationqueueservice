using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using AInBox.Astove.Core.Model;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Web.Hosting;

namespace AInBox.Astove.Core.Util
{
    public class FileManager
    {
        public HttpPostedFileBase File { get; set; }
        public string Directory { get; set; }

        public FileManager() { }
        
        public FileManager(HttpPostedFileBase file, string directory)
        {
            this.File = file;
            this.Directory = directory;
        }

        public Dictionary<string, string> Upload()
        {
            Dictionary<string, string> result = null;

            try
            {
                var fileName = Guid.NewGuid().ToString();
                var extension = (File.ContentType.Contains("jpeg")) ? ".jpg" : ".png";
                if (string.IsNullOrEmpty(File.FileName))
                    extension = Path.GetExtension(File.FileName);
                fileName = string.Format("{0}{1}", Regex.Replace(fileName, @"\s|\$|\#\%", ""), extension);
                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/files/{0}", string.Join("/", this.Directory))), fileName);
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                file.Directory.Create();

                var relativePath = Path.Combine(string.Format(@"\files\{0}", string.Join("\\", this.Directory)), fileName);
                File.SaveAs(path);

                result = new Dictionary<string,string>();
                result.Add("filename", fileName);
                result.Add("url", relativePath.Replace("\\", "/"));
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static bool RemoveFile(string url)
        {
            bool result = false;

            try
            {
                var path = System.Web.HttpContext.Current.Server.MapPath(string.Format("~{0}", url));
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                file.Delete();

                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static void SaveFileFromBytes(string path, byte[] filebytes)
        {
            System.IO.FileStream oFileStream = new System.IO.FileStream(path, System.IO.FileMode.Create);
            oFileStream.Write(filebytes, 0, filebytes.Length);
            oFileStream.Close();
            oFileStream.Dispose();
        }

        public static void HtpClientUpload(string url, string path, string tokenAccess)
        {
            var filename = Path.GetFileName(path);
            var fileStream = new FileStream(HostingEnvironment.MapPath(path), FileMode.Open, FileAccess.Read);

            HtpClientUpload(url, filename, fileStream, tokenAccess);
        }

        public static HttpResponseMessage HtpClientUpload(string url, string filename, Stream fileStream, string tokenAccess)
        {
            HttpContent stringContent = new StringContent(filename);
            HttpContent fileStreamContent = new StreamContent(fileStream);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Concat("Basic ", tokenAccess));
                formData.Add(fileStreamContent, "file", filename);

                var response = client.PostAsync(url, formData).ContinueWith(t => t.Result).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
        }

        public static void HtpClientUpload(string url, string filename, byte[] fileBytes, string tokenAccess)
        {
            HttpContent stringContent = new StringContent(filename);
            HttpContent bytesContent = new ByteArrayContent(fileBytes);

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Concat("Basic ", tokenAccess));
                formData.Add(bytesContent, "file", filename);

                var response = client.PostAsync(url, formData).Result;
            }
        }
    }
}
