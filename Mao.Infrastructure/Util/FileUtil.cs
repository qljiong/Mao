using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Infrastructure.Util
{
    public static class FileUtil
    {
        public static bool IsFileInUse(string fileName)
        {
            bool inUse;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;
            }
            catch
            {
                inUse = true;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return inUse;
        }

        public static string GetFilter(string[] extentions, bool all = false)
        {
            List<string> types = extentions.Select(fileType => string.Format("{0} files(*.{0})|*.{0}", fileType)).ToList();
            if (all) types.Add("All files (*.*)|*.*");
            var filter = string.Join("|", types);
            return filter;
        }

        // 文件是否存在（包含bin等其他目录）
        public static bool FileExist(string filePath)
        {
            string finalPath = _checkAndFillUpPathNoThrow(filePath);
            if (!string.IsNullOrEmpty(finalPath))
            {
                LogUtil.Default.InfoFormat("Found file '{0}'.", finalPath);
                return true;
            }
            LogUtil.Default.InfoFormat("File '" + filePath + "' does not exist.");
            return false;
        }


        // 检查目录下文件是否存在，不存在再去RelativeSearchPath搜索。返回搜索到的全路径,""代表路径不存在
        public static string CheckAndFillUpPath(string filePath)
        {

            string finalPath = _checkAndFillUpPathNoThrow(filePath);

            if (string.IsNullOrEmpty(finalPath) || !FileExist(finalPath))
            {
                throw new FileNotFoundException("File '" + filePath + "' does not exist.");
            }
            return finalPath;
        }

        private static string _checkAndFillUpPathNoThrow(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("filePath could not be null or empty!");
            }

            string originalPath = _searchOriginal(filePath);
            if (!string.IsNullOrEmpty(originalPath))
                return originalPath;

            string baseDirPath = _searchBaseDirectory(filePath);
            if (!string.IsNullOrEmpty(baseDirPath))
                return baseDirPath;

            string relativePath = _searchRelativeSearchPath(filePath);
            if (!string.IsNullOrEmpty(relativePath))
                return relativePath;

            return string.Empty;
        }

        private static string _searchOriginal(string originalFilePath)
        {
            if (File.Exists(originalFilePath))
                return originalFilePath; //存在
            LogUtil.Default.InfoFormat("Could not found file '{0}' in origin path.", originalFilePath);
            return "";
        } //原路径查询

        private static string _searchBaseDirectory(string originalFilePath)
        {
            string otherDirctory = AppDomain.CurrentDomain.BaseDirectory;
            string addedPath = !string.IsNullOrEmpty(otherDirctory) ? otherDirctory + originalFilePath : originalFilePath;
            if (File.Exists(addedPath))
                return addedPath;
            LogUtil.Default.InfoFormat("Could not found file '{0}' in BaseDirectory.", addedPath);
            return "";
        } //运行目录路径查询

        private static string _searchRelativeSearchPath(string originalFilePath)
        {
            string binDircetory = AppDomain.CurrentDomain.RelativeSearchPath;
            string addedPath = !string.IsNullOrEmpty(binDircetory) ?
                binDircetory + @"\" + originalFilePath : originalFilePath;//bin 目录需要多加一个 \ 

            if (File.Exists(addedPath))
                return addedPath;
            LogUtil.Default.InfoFormat("Could not found file '{0}' in RelativeSearchPath.", addedPath);
            return "";
        } //相关搜索路径查询：如web项目的bin目录
    }
}
