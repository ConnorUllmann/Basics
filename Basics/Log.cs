using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Basics
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Breaks a FileInfo's path into the directory, filename, and extension components which combine to form the full path
        /// </summary>
        /// <param name="f">FileInfo whose path will be split</param>
        /// <returns>(directory, filename, extension) which, when combined, form the full path to the file.</returns>
        public static (string directory, string filename, string extension) PathParts(this System.IO.FileInfo f)
            => (f.Directory + "\\", f.Name.Substring(0, f.Name.Length - f.Extension.Length), f.Extension);
    }

    public class Log
    {
        public enum Level
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        };

        private static Log log;

        public bool PrintToConsole;
        public Level Threshold;
        public int MaxLines;

        private string directory = null;
        private string filename = null;
        private string extension = null;
        private bool canWriteToFile => directory != null && filename != null && extension != null;
        private Queue<(string log, Level level, DateTime time)> logs;


        public Log(string _filepath=null, bool _printToConsole=false, Level _threshold=Level.Debug, int _maxLines=100000)
        {
            PrintToConsole = _printToConsole;
            Threshold = _threshold;
            MaxLines = _maxLines;

            if (_filepath != null)
                (directory, filename, extension) = new FileInfo(_filepath).PathParts();

            log = this;
            logs = new Queue<(string, Level, DateTime)>();
        }

        private void insert(string _line, Level _level)
        {
            if (_level < Threshold)
                return;

            if (PrintToConsole)
                Console.WriteLine(_line);

            if (canWriteToFile)
            {
                logs.Enqueue((_line, _level, DateTime.Now));
                if (logs.Count >= MaxLines)
                    Flush();
            }
        }

        public void Flush()
        {
            if (canWriteToFile)
            {
                var filepath = $"{directory}{filename}.{DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss")}{extension}";
                var lines = new List<string>(logs.Select(o => $"[{o.time}][{o.level}] {o.log}"));
                Directory.CreateDirectory(directory);
                File.WriteAllLines(filepath, lines);
            }
            logs.Clear();
        }

        public void Debug(string line) => insert(line, Level.Debug);
        public void Info(string line) => insert(line, Level.Info);
        public void Warning(string line) => insert(line, Level.Warning);
        public void Error(string line) => insert(line, Level.Error);
        public void Critical(string line) => insert(line, Level.Critical);
    }
}
