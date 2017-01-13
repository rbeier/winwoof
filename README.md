	          _                           __ 
	         (_)                         / _|
	__      ___ _ ____      _____   ___ | |_ 
	\ \ /\ / / | '_ \ \ /\ / / _ \ / _ \|  _|
	 \ V  V /| | | | \ V  V / (_) | (_) | |  
	  \_/\_/ |_|_| |_|\_/\_/ \___/ \___/|_|  

### Winwoof is a c# port of [woof](http://www.home.unix-ag.org/simon/woof.html) for Windows host systems.

(win)woof creates a small and simple webserver that can be used to share files or folders easily with people on the same network. 

#### Installation

+ Download the [winwoof.exe](https://github.com/rbeier/winwoof/raw/master/winwoof.exe) from this repository and put it anywhere on your computer
+ Add the path to this file into your PATH variable

#### Usage

You can display a help message by executing winwoof without arguments. To share a file or a folder just pass it as the first argument to winwoof. Alternatively you can specify a port after the file. The path can be relative or absolute.

    winwoof <file|folder> [port]

- - - -

#### License

Copyright © 2017 rbeier  

This work is free. You can redistribute it and/or modify it under the
terms of the Do What The Fuck You Want To Public License, Version 2,
as published by Sam Hocevar. See http://www.wtfpl.net/ for more details.