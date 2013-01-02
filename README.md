process_monitor
===============

Process Monitor for computers in a DOMAIN or LAN. This application can be used to monitor process running on all the PC in a Domain/LAN Network. This process collects data in every 1 hour(Default), which can be customized as per need. All the data collected from the machines are consolidated in a shared directory. To collect data this application has to be installed on all the clients, currently it work only for windows machine.
This application can be used to monitor if any PC in the domain is running any malicious process or can be used to track user activity.

<h1>Windows Desktop Process Monitoring Tool</h1>

DESCRIPTION:
This Service Use Powershell cmdlet to get all the process currently running in a system and log the same on local as well as remote PC
<hr>

<h2>How to configure Path and interval</h2>
<hr>
Go to "Process Monitor" directory

OPEN Process Monitor.exe.config and 
  		Set the following parameters under app settings

			Key =interval 
			value="60" [In minutes]

			Key =serverDir 
			value="\\192.168.1.4\process_list" [Path of a server storage drive where you want the result to be stored]

			Key =localDir 
			value="C:\Program Files\CodeScape"

Note: [Do not change Keys name]
<hr>

<h2>How to Install</h2>
<hr>
Double Click setup file CSPM.msi and follow the steps 
Service will start automatically once installed
<hr>


<h2>HOW IT WORKS</h2>
<hr>
This will execute Get-Process command of powershell after a specific 
interval to get the list of process executing by users of that PC

The output is stored in CSV format and will be stored on local PC and Remote PC
<hr>
