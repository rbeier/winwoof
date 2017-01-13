	        			          _                           __ 
	        			         (_)                         / _|
	        			__      ___ _ ____      _____   ___ | |_ 
	        			\ \ /\ / / | '_ \ \ /\ / / _ \ / _ \|  _|
	        			 \ V  V /| | | | \ V  V / (_) | (_) | |  
	        			  \_/\_/ |_|_| |_|\_/\_/ \___/ \___/|_|  

#### Winwoof is a c# port of [woof](http://www.home.unix-ag.org/simon/woof.html) for Windows host systems.

Woof is a small and simple webserver that can be used to share files or folders easily with people on the same network. 

##### Installation

+ Download the `winwoof.exe` from this repository and put it anywhere on your computer
+ Add the path to this file into your PATH variable

##### Usage

You can display a help message by executing winwoof.exe without arguments. To share a file or a folder just pass it as the first argument to winwoof. Alternatively you can specify a port after the file.

    winwoof <file|folder> [port]