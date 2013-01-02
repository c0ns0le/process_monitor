=============================================CODESCAPE PROCESS MONITOR=================================================================
DESCRIPTION:
This Service Use Powershell cmdlet to get all the process currently running in a system and log the same on local as well as remote PC
----------------------------------------------------------------------------------------------------------------------------------------

			How to configure Path and interval
______________________________________________________________ 
Go to "C:\Program Files\CodeScape\CodeScape Process Monitor" 
						 
OPEN Process Monitor.exe.config and 
			Set the following parameters under app settings
						 
			Key =interval 
			value="60" [In minutes]
						 
			Key =serverDir 
			value="\\192.168.1.4\wamp\www\process_list"
						 
			Key =localDir 
			value="C:\Program Files\CodeScape"
						 
Note: [Do not change Keys name]
_____________________________________________________________


			How to Install
_____________________________________________________________
Double Click setup file CSPM.msi and follow the steps 
Service will start automatically once installed
_____________________________________________________________


			HOW IT WORKS
______________________________________________________________________________

This will execute Get-Process command of powershell after a specific 
interval to get the list of process executing by users of that PC

The output is stored in CSV format and will be stored on local PC and Remote PC
_______________________________________________________________________________