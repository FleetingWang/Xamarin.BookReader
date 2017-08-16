using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.BookReader.Bases;
using Java.IO;
using Java.Lang;
using Java.Util.Zip;
using Java.Util;
using StringBuilder = System.Text.StringBuilder;
using Java.Nio.Channels;
using Java.Text;

namespace Xamarin.BookReader.Utils
{
    public class FileUtils
    {
        public static string getChapterPath(string bookId, int chapter)
        {
            return Constant.PATH_TXT + bookId + File.Separator + chapter + ".txt";
        }

        public static File getChapterFile(string bookId, int chapter)
        {
            File file = new File(getChapterPath(bookId, chapter));
            if (!file.Exists())
                createFile(file);
            return file;
        }

        public static File getBookDir(string bookId)
        {
            return new File(Constant.PATH_TXT + bookId);
        }

        public static File createWifiTempFile()
        {
            string src = Constant.PATH_DATA + "/" + JavaSystem.CurrentTimeMillis();
            File file = new File(src);
            if (!file.Exists())
                createFile(file);
            return file;
        }

        /**
         * 获取Wifi传书保存文件
         *
         * @param fileName
         * @return
         */
        public static File createWifiTranfesFile(string fileName)
        {
            LogUtils.i("wifi trans save " + fileName);
            // 取文件名作为文件夹（bookid）
            string absPath = Constant.PATH_TXT + "/" + fileName + "/1.txt";

            File file = new File(absPath);
            if (!file.Exists())
                createFile(file);
            return file;
        }

        public static string getEpubFolderPath(string epubFileName)
        {
            return Constant.PATH_EPUB + "/" + epubFileName;
        }

        public static string getPathOPF(string unzipDir)
        {
            string mPathOPF = "";
            try
            {
                BufferedReader br = new BufferedReader(new InputStreamReader(new System.IO.FileStream(unzipDir
                        + "/META-INF/container.xml", System.IO.FileMode.Open), "UTF-8"));
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Contains("full-path"))
                    {
                        int start = line.IndexOf("full-path");
                        int start2 = line.IndexOf('\"', start);
                        int stop2 = line.IndexOf('\"', start2 + 1);
                        if (start2 > -1 && stop2 > start2)
                        {
                            mPathOPF = line.Substring(start2 + 1, stop2).Trim();
                            break;
                        }
                    }
                }
                br.Close();

                if (!mPathOPF.Contains("/"))
                {
                    return null;
                }

                int last = mPathOPF.LastIndexOf('/');
                if (last > -1)
                {
                    mPathOPF = mPathOPF.Substring(0, last);
                }

                return mPathOPF;
            }
            catch (Exception e)
            {
                LogUtils.e(e.ToString());
            }
            return mPathOPF;
        }

        public static bool checkOPFInRootDirectory(string unzipDir)
        {
            string mPathOPF = "";
            bool status = false;
            try
            {
                BufferedReader br = new BufferedReader(new InputStreamReader(new System.IO.FileStream(unzipDir
                        + "/META-INF/container.xml", System.IO.FileMode.Open), "UTF-8"));
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    if (line.Contains("full-path"))
                    {
                        int start = line.IndexOf("full-path");
                        int start2 = line.IndexOf('\"', start);
                        int stop2 = line.IndexOf('\"', start2 + 1);
                        if (start2 > -1 && stop2 > start2)
                        {
                            mPathOPF = line.Substring(start2 + 1, stop2).Trim();
                            break;
                        }
                    }
                }
                br.Close();

                if (!mPathOPF.Contains("/"))
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception e)
            {
                LogUtils.e(e.ToString());
            }
            return status;
        }

        public static void unzipFile(string inputZip, string destinationDirectory)
        {

            int buffer = 2048;
            List<string> zipFiles = new List<string>();
            File sourceZipFile = new File(inputZip);
            File unzipDirectory = new File(destinationDirectory);

            createDir(unzipDirectory.AbsolutePath);

            ZipFile zipFile;
            zipFile = new ZipFile(sourceZipFile, ZipFile.OpenRead);
            IEnumeration zipFileEntries = zipFile.Entries();

            while (zipFileEntries.HasMoreElements)
            {

                ZipEntry entry = (ZipEntry)zipFileEntries.NextElement();
                string currentEntry = entry.Name;
                File destFile = new File(unzipDirectory, currentEntry);

                if (currentEntry.EndsWith(Constant.SUFFIX_ZIP))
                {
                    zipFiles.Add(destFile.AbsolutePath);
                }

                File destinationParent = destFile.ParentFile;
                createDir(destinationParent.AbsolutePath);

                if (!entry.IsDirectory)
                {

                    if (destFile != null && destFile.Exists())
                    {
                        LogUtils.i(destFile + "已存在");
                        continue;
                    }

                    BufferedInputStream inputStream = new BufferedInputStream(zipFile.GetInputStream(entry));
                    int currentByte;
                    // buffer for writing file
                    byte[] data = new byte[buffer];

                    var fos = new System.IO.FileStream(destFile.AbsolutePath, System.IO.FileMode.OpenOrCreate);
                    BufferedOutputStream dest = new BufferedOutputStream(fos, buffer);

                    while ((currentByte = inputStream.Read(data, 0, buffer)) != -1)
                    {
                        dest.Write(data, 0, currentByte);
                    }
                    dest.Flush();
                    dest.Close();
                    inputStream.Close();
                }
            }
            zipFile.Close();

            foreach (var zipName in zipFiles)
            {
                unzipFile(zipName, destinationDirectory + File.SeparatorChar
                        + zipName.Substring(0, zipName.LastIndexOf(Constant.SUFFIX_ZIP)));
            }
        }

        /**
         * 读取Assets文件
         *
         * @param fileName
         * @return
         */
        public static byte[] readAssets(string fileName)
        {
            if (fileName == null || fileName.Length <= 0)
            {
                return null;
            }
            byte[] buffer = null;
            try
            {
                var fin = AppUtils.getAppContext().Assets.Open("uploader" + fileName);
                buffer = new byte[fin.Length];
                fin.Read(buffer, 0, (int)fin.Length);
                fin.Close();
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return buffer;
        }

        /**
         * 创建根缓存目录
         *
         * @return
         */
        public static string createRootPath(Context context)
        {
            string cacheRootPath = "";
            if (isSdCardAvailable())
            {
                // /sdcard/Android/data/<application package>/cache
                cacheRootPath = context.ExternalCacheDir.Path;
            }
            else
            {
                // /data/data/<application package>/cache
                cacheRootPath = context.CacheDir.Path;
            }
            return cacheRootPath;
        }

        public static bool isSdCardAvailable()
        {
            return Environment.MediaMounted.Equals(Environment.ExternalStorageState);
        }

        /**
         * 递归创建文件夹
         *
         * @param dirPath
         * @return 创建失败返回""
         */
        public static string createDir(string dirPath)
        {
            try
            {
                File file = new File(dirPath);
                if (file.ParentFile.Exists())
                {
                    LogUtils.i("----- 创建文件夹" + file.AbsolutePath);
                    file.Mkdir();
                    return file.AbsolutePath;
                }
                else
                {
                    createDir(file.ParentFile.AbsolutePath);
                    LogUtils.i("----- 创建文件夹" + file.AbsolutePath);
                    file.Mkdir();
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return dirPath;
        }

        /**
         * 递归创建文件夹
         *
         * @param file
         * @return 创建失败返回""
         */
        public static string createFile(File file)
        {
            try
            {
                if (file.ParentFile.Exists())
                {
                    LogUtils.i("----- 创建文件" + file.AbsolutePath);
                    file.CreateNewFile();
                    return file.AbsolutePath;
                }
                else
                {
                    createDir(file.ParentFile.AbsolutePath);
                    file.CreateNewFile();
                    LogUtils.i("----- 创建文件" + file.AbsolutePath);
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return "";
        }

        /**
         * 将内容写入文件
         *
         * @param filePath eg:/mnt/sdcard/demo.txt
         * @param content  内容
         * @param isAppend 是否追加
         */
        public static void writeFile(string filePath, string content, bool isAppend)
        {
            LogUtils.i("save:" + filePath);
            try
            {
                FileOutputStream fout = new FileOutputStream(filePath, isAppend);
                byte[] bytes = Encoding.UTF8.GetBytes(content);
                fout.Write(bytes);
                fout.Close();
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
        }

        public static void writeFile(string filePathAndName, string fileContent)
        {
            try
            {
                var outstream = new System.IO.FileStream(filePathAndName, System.IO.FileMode.OpenOrCreate);
                OutputStreamWriter outSW = new OutputStreamWriter(outstream);
                outSW.Write(fileContent);
                outSW.Close();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }

        /**
         * 获取Raw下的文件内容
         *
         * @param context
         * @param resId
         * @return 文件内容
         */
        public static string getFileFromRaw(Context context, int resId)
        {
            if (context == null)
            {
                return null;
            }

            StringBuilder s = new StringBuilder();
            try
            {
                InputStreamReader inStream = new InputStreamReader(context.Resources.OpenRawResource(resId));
                BufferedReader br = new BufferedReader(inStream);
                string line;
                while ((line = br.ReadLine()) != null)
                {
                    s.Append(line);
                }
                return s.ToString();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
                return null;
            }
        }

        public static byte[] getBytesFromFile(File f)
        {
            if (f == null)
            {
                return null;
            }
            try
            {
                FileInputStream stream = new FileInputStream(f);
                ByteArrayOutputStream outStream = new ByteArrayOutputStream(1000);
                byte[] b = new byte[1000];
                for (int n; (n = stream.Read(b)) != -1;)
                {
                    outStream.Write(b, 0, n);
                }
                stream.Close();
                outStream.Close();
                return outStream.ToByteArray();
            }
            catch (IOException e)
            {
            }
            return null;
        }

        /**
         * 文件拷贝
         *
         * @param src  源文件
         * @param desc 目的文件
         */
        public static void fileChannelCopy(File src, File desc)
        {
            //createFile(src);
            createFile(desc);
            FileInputStream fi = null;
            FileOutputStream fo = null;
            try
            {
                fi = new FileInputStream(src);
                fo = new FileOutputStream(desc);
                FileChannel inStream = fi.Channel;//得到对应的文件通道
                FileChannel outStream = fo.Channel;//得到对应的文件通道
                inStream.TransferTo(0, inStream.Size(), outStream);//连接两个通道，并且从in通道读取，然后写入out通道
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                try
                {
                    if (fo != null) fo.Close();
                    if (fi != null) fi.Close();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }
            }
        }

        /**
         * 转换文件大小
         *
         * @param fileLen 单位B
         * @return
         */
        public static string formatFileSizeToString(long fileLen)
        {
            DecimalFormat df = new DecimalFormat("0.00");
            string fileSizestring = "";
            if (fileLen < 1024)
            {
                fileSizestring = df.Format((double)fileLen) + "B";
            }
            else if (fileLen < 1048576)
            {
                fileSizestring = df.Format((double)fileLen / 1024) + "K";
            }
            else if (fileLen < 1073741824)
            {
                fileSizestring = df.Format((double)fileLen / 1048576) + "M";
            }
            else
            {
                fileSizestring = df.Format((double)fileLen / 1073741824) + "G";
            }
            return fileSizestring;
        }

        /**
         * 删除指定文件
         *
         * @param file
         * @return
         * @throws IOException
         */
        public static bool deleteFile(File file)
        {
            return deleteFileOrDirectory(file);
        }

        /**
         * 删除指定文件，如果是文件夹，则递归删除
         *
         * @param file
         * @return
         * @throws IOException
         */
        public static bool deleteFileOrDirectory(File file)
        {
            try
            {
                if (file != null && file.IsFile)
                {
                    return file.Delete();
                }
                if (file != null && file.IsDirectory)
                {
                    File[] childFiles = file.ListFiles();
                    // 删除空文件夹
                    if (childFiles == null || childFiles.Length == 0)
                    {
                        return file.Delete();
                    }
                    // 递归删除文件夹下的子文件
                    for (int i = 0; i < childFiles.Length; i++)
                    {
                        deleteFileOrDirectory(childFiles[i]);
                    }
                    return file.Delete();
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return false;
        }

        /**
         * 获取文件夹大小
         *
         * @return
         * @throws Exception
         */
        public static long getFolderSize(string dir)
        {
            File file = new File(dir);
            long size = 0;
            try
            {
                File[] fileList = file.ListFiles();
                for (int i = 0; i < fileList.Length; i++)
                {
                    // 如果下面还有文件
                    if (fileList[i].IsDirectory)
                    {
                        size = size + getFolderSize(fileList[i].AbsolutePath);
                    }
                    else
                    {
                        size = size + fileList[i].Length();
                    }
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            return size;
        }

        /***
         * 获取文件扩展名
         *
         * @param filename 文件名
         * @return
         */
        public static string getExtensionName(string filename)
        {
            if ((filename != null) && (filename.Length > 0))
            {
                int dot = filename.LastIndexOf('.');
                if ((dot > -1) && (dot < (filename.Length - 1)))
                {
                    return filename.Substring(dot + 1);
                }
            }
            return filename;
        }

        /**
         * 获取文件内容
         *
         * @param path
         * @return
         */
        public static string getFileOutputString(string path, string charset)
        {
            try
            {
                File file = new File(path);
                BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(new System.IO.FileStream(file.AbsolutePath, System.IO.FileMode.Open), charset), 8192);
                StringBuilder sb = new StringBuilder();
                string line = null;
                while ((line = bufferedReader.ReadLine()) != null)
                {
                    sb.Append("\n").Append(line);
                }
                bufferedReader.Close();
                return sb.ToString();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
            return null;
        }

        /**
         * 递归获取所有文件
         *
         * @param root
         * @param ext  指定扩展名
         */
        private /*synchronized*/ void getAllFiles(File root, string ext)
        {
            List<File> list = new List<File>();
            File[] files = root.ListFiles();
            if (files != null)
            {
                foreach (File f in files)
                {
                    if (f.IsDirectory)
                    {
                        getAllFiles(f, ext);
                    }
                    else
                    {
                        if (f.Name.EndsWith(ext) && f.Length() > 50)
                            list.Add(f);
                    }
                }
            }
        }

        public static string getCharset(string fileName)
        {
            BufferedInputStream bis = null;
            string charset = "GBK";
            byte[] first3Bytes = new byte[3];
            try
            {
                bool check = false;
                bis = new BufferedInputStream(new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate));
                bis.Mark(0);
                int read = bis.Read(first3Bytes, 0, 3);
                if (read == -1)
                    return charset;
                if (first3Bytes[0] == (byte)0xFF && first3Bytes[1] == (byte)0xFE)
                {
                    charset = "UTF-16LE";
                    check = true;
                }
                else if (first3Bytes[0] == (byte)0xFE
                      && first3Bytes[1] == (byte)0xFF)
                {
                    charset = "UTF-16BE";
                    check = true;
                }
                else if (first3Bytes[0] == (byte)0xEF
                      && first3Bytes[1] == (byte)0xBB
                      && first3Bytes[2] == (byte)0xBF)
                {
                    charset = "UTF-8";
                    check = true;
                }
                bis.Mark(0);
                if (!check)
                {
                    while ((read = bis.Read()) != -1)
                    {
                        if (read >= 0xF0)
                            break;
                        if (0x80 <= read && read <= 0xBF) // 单独出现BF以下的，也算是GBK
                            break;
                        if (0xC0 <= read && read <= 0xDF)
                        {
                            read = bis.Read();
                            if (0x80 <= read && read <= 0xBF) // 双字节 (0xC0 - 0xDF)
                                                              // (0x80 - 0xBF),也可能在GB编码内
                                continue;
                            else
                                break;
                        }
                        else if (0xE0 <= read && read <= 0xEF)
                        {// 也有可能出错，但是几率较小
                            read = bis.Read();
                            if (0x80 <= read && read <= 0xBF)
                            {
                                read = bis.Read();
                                if (0x80 <= read && read <= 0xBF)
                                {
                                    charset = "UTF-8";
                                    break;
                                }
                                else
                                    break;
                            }
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                if (bis != null)
                {
                    try
                    {
                        bis.Close();
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                }
            }

            return charset;
        }

        public static string getCharset1(string fileName)
        {
            BufferedInputStream bin = new BufferedInputStream(new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate));
            int p = (bin.Read() << 8) + bin.Read();

            string code;
            switch (p)
            {
                case 0xefbb:
                    code = "UTF-8";
                    break;
                case 0xfffe:
                    code = "Unicode";
                    break;
                case 0xfeff:
                    code = "UTF-16BE";
                    break;
                default:
                    code = "GBK";
                    break;
            }
            return code;
        }

        public static void saveWifiTxt(string src, string desc)
        {
            byte[] LINE_END = Encoding.ASCII.GetBytes("\n");
            try
            {
                InputStreamReader isr = new InputStreamReader(new System.IO.FileStream(src, System.IO.FileMode.OpenOrCreate), getCharset(src));
                BufferedReader br = new BufferedReader(isr);

                FileOutputStream fout = new FileOutputStream(desc, true);
                string temp;
                while ((temp = br.ReadLine()) != null)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(temp);
                    fout.Write(bytes);
                    fout.Write(LINE_END);
                }
                br.Close();
                fout.Close();
            }
            catch (FileNotFoundException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}