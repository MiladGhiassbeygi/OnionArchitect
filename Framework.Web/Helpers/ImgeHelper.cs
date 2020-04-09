using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Web.Helpers
{
    public class ImageHelper
    {
        private IHostingEnvironment CurrentEnvironment;
        public ImageHelper()
        {

        }

        public ImageHelper(IHostingEnvironment env)
        {
            CurrentEnvironment = env;
        }
        public string ServerMapped()
        {

            string envName = CurrentEnvironment.WebRootPath;

            return envName;
        }
        public string SaveFile(IFormFile Temp, string SubPath)
        {
            string Filepath = "\\Images" + "\\" + SubPath + "\\";
            if (Temp == null)
            {
                return "NULL";
            }
            string TempFullName = Temp.FileName;
            string TempSuffix = Path.GetExtension(TempFullName);

            Stream stream = Temp.OpenReadStream();

            byte[] imageBytes = new byte[stream.Length];
            using (Image img = Image.FromStream(stream))
            {
                int h = 1000;
                int w = 1000;

                using (Bitmap b = new Bitmap(img, new Size(w, h)))
                {
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                        imageBytes = ms2.ToArray();
                    }
                }
            }
            string path = ServerMapped() + Filepath + Guid.NewGuid().ToString().Substring(0, 22) + TempSuffix;
            File.WriteAllBytes(path, imageBytes);
            path = path.Substring(path.LastIndexOf("Images"));
            path = path.Replace('\\', '/');
            return path;
        }
        public async Task<Hashtable> FroalaImagerSaver(List<IFormFile> files, IFormFile theFile, string mimeType, string subPath = "froala")
        {
            string webRootPath = ServerMapped();
            var fileRoute = webRootPath + "\\Images\\" + subPath + "\\";
            string extension = System.IO.Path.GetExtension(theFile.FileName);
            string name = Guid.NewGuid().ToString().Substring(0, 8) + extension;
            string link = Path.Combine(fileRoute, name);
            FileInfo dir = new FileInfo(fileRoute);
            dir.Directory.Create();
            string[] imageMimetypes = { "image/gif", "image/jpeg", "image/pjpeg", "image/x-png", "image/png", "image/svg+xml" };
            string[] imageExt = { ".gif", ".jpeg", ".jpg", ".png", ".svg", ".blob" };
            try
            {
                if (Array.IndexOf(imageMimetypes, mimeType) >= 0 && (Array.IndexOf(imageExt, extension) >= 0))
                {
                    // Copy contents to memory stream.
                    Stream stream;
                    stream = new MemoryStream();
                    theFile.CopyTo(stream);
                    stream.Position = 0;
                    String serverPath = link;

                    // Save the file
                    using (FileStream writerFileStream = System.IO.File.Create(serverPath))
                    {
                        await stream.CopyToAsync(writerFileStream);
                        writerFileStream.Dispose();
                    }

                    // Return the file path as json
                    Hashtable imageUrl = new Hashtable();
                    serverPath = serverPath.Substring(serverPath.LastIndexOf("Images") - 1);

                    imageUrl.Add("link", serverPath);

                    return imageUrl;
                }
                throw new ArgumentException("The image did not pass the validation");
            }

            catch (ArgumentException ex)
            {
                Hashtable error = new Hashtable();
                error.Add("Error", ex.Message);
                return error;
            }
        }
        public void DeleteFile(string FilePath)
        {
            string ServerPath = ServerMapped();
            string FullPath = ServerPath + "\\" + FilePath;
            File.Delete(FullPath);
        }

    }

}
