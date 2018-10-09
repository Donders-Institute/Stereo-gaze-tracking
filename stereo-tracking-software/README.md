# Stereo tracking software

Stereo-gaze tracking software is developed in Visual Studio 2012 with C# as the programming language and was developed at the Donders Institute for Brain, Cognition and Behaviour by Annemiek Barsingerhorn and Jeroen Goossens.

The stereo eyetracker software and the gaze reconstruction algorithms are described in the following paper, which also includes a description of the hardware:

Barsingerhorn, Boonstra & Goossens (2018). Development and validation of a high-speed stereoscopic eyetracker. Behavior Research Methods. DOI: 10.3758/s13428-018-1026-7

At present the software only works with Lumenera cameras, but it should be fairly easy to get it to work with cameras from other suppliers as well. There is room for improving the software on other aspects as well. The software was developen under Windows 7, but also works on Windows 10.

The logfiles are saved in the following directory: "C:\\Users\\Public\\Roaming\\StereoEyetracker". The gaze reconstruction is performed offline, examples and m-files can be found in the folder gaze-reconstruction within this repository. 

The Stereo-gaze-tracking software is free but copyrighted software, distributed under the terms of the GNU General Public Licence as published by the Free Software Foundation (either version 3, or at your option any later version). See the file LICENSE for more details.
