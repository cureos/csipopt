// Copyright (c) 2011 Anders Gustafsson, Cureos AB.
// All rights reserved. This software and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace Cureos.Utility
{
    public static class NativeDllHelper
    {
        private static readonly string _nativeDllDirectory;
        private static readonly string _executingAssembly;

        /// <summary>
        /// Static constructor for defining the path to the native DLL directory and the short name 
        /// of the executing assembly 
        /// </summary>
        static NativeDllHelper()
        {
            _nativeDllDirectory = String.Format(CultureInfo.InvariantCulture, @"{0}{1}Silverlight{1}Native",
                                                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                                Path.DirectorySeparatorChar);
            _executingAssembly = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Name;
        }

        /// <summary>
        /// Setup native DLL directory and include it in system path, then add specified resource DLLs to directory
        /// </summary>
        /// <param name="dllPaths">Relative file paths to the DLL resources in the application project</param>
        public static void SetupNativeDllFolder(params string[] dllPaths)
        {
            try
            {
                // Create native DLL directory if not already existing
                if (!Directory.Exists(_nativeDllDirectory)) Directory.CreateDirectory(_nativeDllDirectory);
            }
            catch (IOException)
            {
                return;
            }

            const int size = 32767;
            const string path = "PATH";
            var buffer = new StringBuilder(size);

            // Add local DLL directory to the PATH environment variable during execution
            GetEnvironmentVariable(path, buffer, size);
            buffer.Insert(0, ";");
            buffer.Insert(0, _nativeDllDirectory);
            SetEnvironmentVariable(path, buffer.ToString());

            // Loop over the listed DLL resources and copy each DLL resource to the native DLL directory
            foreach (var dllPath in dllPaths)
            {
                try
                {
                    var resourceStream = Application.GetResourceStream(GetResourceUri(dllPath));
                    CopyDllToNativeDirectory(resourceStream, dllPath);
                }
                catch (NullReferenceException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        /// <summary>
        /// Create a relative URI to the specified resource file
        /// </summary>
        /// <param name="fileName">Path to the resource file, relative to the application project root</param>
        /// <returns>URI to the resource file in the executing assembly</returns>
        private static Uri GetResourceUri(string fileName)
        {
            return
                new Uri(
                    String.Format(CultureInfo.InvariantCulture, "/{0};component/{1}", _executingAssembly,
                                  fileName.TrimStart('/')),
                    UriKind.Relative);
        }

        /// <summary>
        /// Copy the specified DLL resource file to the native DLL directory
        /// </summary>
        /// <param name="resourceInfo">Info object for the DLL resource stream</param>
        /// <param name="fileName">Requested name of the resource file; any specified directory paths will be 
        /// removed prior to copying</param>
        private static void CopyDllToNativeDirectory(StreamResourceInfo resourceInfo, string fileName)
        {
            using (
                var fileStream =
                    new FileStream(
                        String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", _nativeDllDirectory, Path.DirectorySeparatorChar,
                                      Path.GetFileName(fileName)), FileMode.Create))
            {
                resourceInfo.Stream.CopyTo(fileStream);
            }
        }

        /// <summary>
        /// Retrieves the contents of the specified variable from the environment block of the calling process. 
        /// C# signature was obtained from http://pinvoke.net/default.aspx/kernel32.GetEnvironmentVariable
        /// Full API documentation is available at http://msdn.microsoft.com/en-us/library/ms683188(v=VS.85).aspx
        /// </summary>
        /// <param name="lpName">The name of the environment variable.</param>
        /// <param name="lpBuffer">A pointer to a buffer that receives the contents of the specified environment variable.</param>
        /// <param name="nSize">The size of the buffer pointed to by the lpBuffer parameter.</param>
        /// <returns>If the function succeeds, the return value is the number of characters stored in the buffer pointed 
        /// to by lpBuffer. If the function fails, the return value is zero.</returns>
        [DllImport("kernel32", SetLastError = true)]
        private static extern uint GetEnvironmentVariable(string lpName, [Out] StringBuilder lpBuffer, uint nSize);

        /// <summary>
        /// Sets the contents of the specified environment variable for the current process.
        /// C# signature was obtained from http://pinvoke.net/default.aspx/kernel32.SetEnvironmentVariable
        /// Full API documentation is available at http://msdn.microsoft.com/en-us/library/ms686206(v=VS.85).aspx
        /// </summary>
        /// <param name="lpName">The name of the environment variable. The operating system creates the environment 
        /// variable if it does not exist and lpValue is not NULL.</param>
        /// <param name="lpValue">The contents of the environment variable. If this parameter is NULL, the variable 
        /// is deleted from the current process's environment.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetEnvironmentVariable(string lpName, string lpValue);
    }
}