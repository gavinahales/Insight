Insight
=======

Description
-----------

This application was designed as part of a PhD research project to visualise the output of an Autopsy digital forensics investigation in a 2D timeline format.

It is no longer actively developed, and the code provided is presented 'as-is'. It contains known bugs which are not intended to be fixed.

The thesis which relates to this project can be found here:
https://repository.abertay.ac.uk/jspui/handle/10373/2413

Requirements
------------

This project is designed to run on Windows and targets version 4.5 of the .NET framework.

Licence
-------

This project is distributed under the terms of the GNU GPL v3 licence.

Instructions
------------

To use this application, you must first run an ingest on a disk image using the Autopsy 3 application, which will produce an 'autopsy.db' file.
You should run the InsightPreprocessor application on this file (this is a separate application, hosted in another repo) which will produce an 'insight.xml' file.
Move this file to the same directory as you Insight.exe file, and then run the application. It should detect and load the file automatically.
This isn't a streamlined process, due to this only being a proof-of-concept application which took many different forms!
There are number of dependencies which should be automatically resolved by NuGet when you build the code.