## How can I run CMDR in my macOS?

There is three ways to edit your mappings in macOS.


### Simplest: Xtrememapping

The simplest answer is: buy a copy of [XtremmeMapping](https://www.xtrememapping.com/) for macOS.

### More complex: Azure Cloud

The more complex answer is: get a free trial of a windows virtual machine. 

This will run in the "azure" microsoft cloud, and requires giving your credit card to detail to microsoft 
(they say over and over that no automatic billing will ever happen after the trial period without you changing your )

installation:
* link1: https://microsoft.github.io/AzureTipsAndTricks/blog/tip246.html
* link2: https://www.techrepublic.com/article/build-your-own-vm-in-the-cloud-with-microsoft-azure/

Then connect to your cloud machine using RDP for mac:
https://www.techrepublic.com/article/pro-tip-remote-desktop-on-mac-what-you-need-to-know/

Then:
  * Install CMDR as explained here: [CMDR installation instructions](https://github.com/pestrela/cmdr#download-and-installation)
  * Copy your TSI into the virtual machine (simplest is to use eg google drive on the browser)
 
### Most complex: VirtualBox

The most complex answer is: get a personal virtual machine couples with an evaluation copy of any Windows OS.

Step by step instructions are on:
  * https://towardsdatascience.com/how-to-install-a-free-windows-virtual-machine-on-your-mac-bf7cbc05888e
  * Step 1: Download VirtualBox
  * Step 2: Grab Windows 10
  * Step 3: Install VirtualBox and the extension pack
  * Step 4: Get your OS up and running

Then:
  * Install CMDR as explained here: [CMDR installation instructions](https://github.com/pestrela/cmdr#download-and-installation)
  * Copy your TSI into the virtual machine (simplest is to use eg google drive on the browser)
 