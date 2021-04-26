# ServiceWrapper

A wrapper for commands to be executed as Windows services. Built for planting backdoors during red teaming operations, however some admins and regular users may find it useful too.

Works with .NET Framework 3.5 upwards (i.e. starting with Windows XP). For the sake of simplicity and portability, it is not currently able to track processes and report errors. So, if for any reason a subprocess dies, the service will still be shown as running. I may add proper background processing at some point, however it works pretty well for me as it is.

## Configuration

The configuration file `ServiceWrapper.xml` must be placed in the same directory as the executable.

### Example

```
<?xml version="1.0" encoding="utf-8"?>
<ServiceConfiguration>
    <Name>OrdinaryService</Name>
    <Description>This is nothing interesting, really.</Description>
    <RunAs>LocalSystem</RunAs>
    <Logging>
        <ServiceLogName>OrdinaryName</ServiceLogName>
        <ServiceLogSource>OrdinarySource</ServiceLogSource>
    </Logging>
    <Commands>
        <Command enabled="true">
            <Executable>C:\Users\Public\Documents\Netflip.exe</Executable>
            <Arguments>--connect=C:\Users\Public\Documents\connection-443.xor --key=15</Arguments>
        </Command>
        <Command enabled="true">
            <Executable>C:\Users\Public\Documents\Netflip.exe</Executable>
            <Arguments>--connect=C:\Users\Public\Documents\connection-8888.xor --key=15</Arguments>
        </Command>
    </Commands>
</ServiceConfiguration>
```

### Options

* RunAs
    - LocalSystem
    - LocalService
    - NetworkService
    - User

## Installation

`c:\>C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe ServiceWrapper.exe`
