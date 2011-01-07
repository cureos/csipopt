csipopt
=======
Developed by Anders Gustafsson, anders dot 9ustafsson at gmail dot com.
Published under the Eclipse Public License.

Introduction
------------
csipopt provides a small and simple C# interface to the Ipopt non-linear optimizer. If the interface is
contained in a .NET class library, the interface is accessible via any .NET language. .NET Framework 2.0
or higher is required.

Prerequisites
-------------
To successfully run an application calling the C# Ipopt interface, a dynamic linked library of Ipopt is 
additionally required. Pre-compiled libraries can be downloaded from the following location: 
http://www.coin-or.org/download/binary/Ipopt/

Download binaries suitable for your platform, unpack the compressed Ipopt binary archive, from the folder 
.\lib\PLATFORM\release (where PLATFORM is win32 or x64) copy the Ipopt39.dll file and place it in a folder 
that is accessible from the system path. To run the example applications in this project, place the 
Ipopt39.dll in the same folder as the project source code.

(Note! The actual name of the DLL is of course dependent upon which version of Ipopt libraries you are 
downloading.)

Usage
-----
To use the C# interface in your own code, simply copy the file Ipopt.cs to your class library. 
(Potentially, you may need to edit the references to Ipopt39.) The class library must be compiled 
using the /unsafe directive. Other class libraries and applications accessing the class library with 
the C# Ipopt interface do not require the /unsafe directive.

A number of build scripts are provided to demonstrate example usage of the C# Ipopt interface.

Visual Studio examples
----------------------
When running from the Visual Studio command prompt, create a class library by running the batch script

	build_library

Create an example application from the C# file Program.cs by running the batch script

	build_hs071_cs

There is an alternative C# build script enforcing the intermediate callback method (introduced in 
Ipopt 3.9) to be utilized. To create an example application using the intermediate callback method, run

	build_hs071_cs_with_intermediate

Analogously, create an example application from the Visual Basic file Program.vb or F# file Program.fs
by running either of the batch scripts

	build_hs071_vb
	build_hs071_fs

To run the respective applications, type

	hs071_cs
	hs071_cs_intermediate
	hs071_vb
	hs071_fs

Mono examples
-------------
The interface is equally applicable to Mono, and can be compiled using the gmcs compiler.
When running from the Mono command prompt, create a class library by running the batch script

	build_library_with_mono

Create an example application from the C# file Program.cs by running the batch script

	build_hs071_cs_with_mono

To run the example application, type

	mono hs071_cs


=======
This file was last modified by Anders Gustafsson, 2010-12-08.
