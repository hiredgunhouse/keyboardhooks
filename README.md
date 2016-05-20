Keyboard Hooks
==============

[![Build status](https://ci.appveyor.com/api/projects/status/45cdyx00o17pufu1?svg=true)](https://ci.appveyor.com/project/PiotrOwsiak/keyboardhooks)

This program changes Ctrl to act as either Ctrl (when pressed in combination with other key) or Esc (when pressed and release).
Additionally when Ctrl is pressed and released after specified, configurable timeout no key is sent.

###Usage:  
At startup it minimizes to tray.  
The tray icon has a context menu (*Show* and *Exit* items) and on double click shows the main form.  
Closing the form minimizes to tray.
Calling the program with */enable* parameter activates Ctrl as Esc rather than having to click the *Enable* button (usefull for system autorun shortcuts).

###Warning:  
This program has been done very quickly and "works on my machine" (Windows 7 EN 64-bit and Windows 7 PL 64-bit). It has been tested only on 3 machines.

Pull requests are welcomed.

###Known problems:  
~~Does not work on Windows 7 PL 64-bit (tested on a single machine)~~
Does not seem to work on Windows 10

###TODO:
- ~~icon~~
- ~~minimize to tray~~

###Credits

Icon by Jack Cai - http://www.doublejdesign.co.uk/
