JobSharp
======

An easy-to-use C# jobs runner.

--------

Features
===
 - Individual jobs scheduling using Crontab expressions.
 - Easy selection of what jobs run in each Windows Service instance.
 - Fault Tolerant. An failure in a job will not affect the others jobs running.
 - Auto discovering jobs. Just implement [IJob](src/JobSharp/IJob.cs) interface and your job will be discovered. 
 - Easy installation and command-line via [Topshelf](https://github.com/phatboyg/Topshelf).
 - Run a specific job via -job:&lt;job name&gt; command-line option.
 - Fully tested on Windows.
 - Working on Visual Studio and Xamarin Studio.
 - [Unit tests](src/JobSharp.UnitTests) and [functional tests](src/JobSharp.FunctionalTests) .
 - 100% code documentation.
 - FxCop and StyleCop validated.
 - Good (and well used) design patterns.  

--------

Setup
===

* In VS|XS add a new "Console Application" project to your solution.
* Install-Package JobSharp
* Create a folder called "Jobs" and create your jobs inside it. To create the job, follow the instructions in ["Creating a job"](#Step1)
* Add an "Application Configuration File" (app.config) to your project.
	* Add the key &lt;add key="JobSharp:CronTab" value="1000" /&gt; to the appSettings section.
	* Follow the steps described in ["Configuring the job"](#Step2) 
* Replace the content of Program.cs file with the code described in step ["Configuring Windows Service"](#Step3)
* Configure the jobs logging as describe in ["Configuring the logging"](#Step4)
* Installing Windows Service as describe in ["Installing Windows Service"](#Step5)
* To run the application follow the steps described in ["Running"](#Step6)

<a name="Step1">Creating a job</a>
===
```csharp

class MyFirstJob : IJob
{
    public void Run()
    {
        // Put your job's logic or call your domain logic here.
        LogService.Write("My first job using JobSharp is running ;)");
    }
}

```

<a name="Step2">Configuring the job</a>
===

Add your new job "JobSharp:Jobs" key in the appSettings section (use comma to add more jobs):
```xml

<add key="JobSharp:Jobs" value="MyFirstJob" />

```

Define when the job must run:
```xml

<add key="MyFirstJob:Crontab" value="*/2 * * * *" />

```

This configuration will make the job runs every 2 minutes.


<a name="Step3">Configuring Windows Service</a>
===

Replace the content of the file Program.cs:

```csharp
using JobSharp;
using Skahal.Infrastructure.Logging.Log4net;
using Topshelf;

class Program
{
    static void Main(string[] args)
    {
        WindowsService.Install(
		new Log4netLogStrategy ("JobSharpLog"), 
		c =>
        {
            c.RunAsLocalSystem();

            c.SetDescription("A very simple JobSharp Sample");
            c.SetDisplayName("JobSharp Sample");
            c.SetServiceName("JobSharpSample");
            c.StartAutomatically();
            c.EnableShutdown();
        });        
    }
}

```


<a name="Step4">Configuring the logging</a>
===
The default configuration use Log4net as the log strategy.

To configure log4net take a look on JobSharp.Sample's [App.config](src/JobSharp.Sample/App.Config)

If Log4net does not meet what you need, I doubt ;), you can implement your own ILogStrategy and inject it on WindowsService.Install method call. 


<a name="Step5">Installing Windows Service</a>
===
Open a prompt window on directory with your console application .exe file and type: &lt;your console application name&gt;.exe install

<a name="Step6">Running</a>
===
You can run it as a Windows Service or just as a command-line.

More about command-line options in [Topshelf docs](http://docs.topshelf-project.com/en/latest/overview/commandline.html?highlight=command%20line).


Sample
===
There is a sample project called [JobSharp.Sample](src/JobSharp.Sample) in the solution. This sample has the basic bootstrap for a project using JobSharp.

You can [fork](https://github.com/giacomelli/JobSharp/fork) the full source code to see how a JobSharp powered Windows Service looks like.

--------

FAQ
-------- 
Having troubles? 
 
- Ask on [Stack Overflow](http://stackoverflow.com/search?q=JobSharp)

Roadmap
-------- 
 - Create a configuration section handler to move the configuration from appSettings section to a specific JobSharp section.
 
--------

How to improve it?
======

- Create a fork of [JobSharp](https://github.com/giacomelli/JobSharp/fork). 
- Did you change it? [Submit a pull request](https://github.com/giacomelli/JobSharp/pull/new/master).


License
======

Licensed under the The MIT License (MIT).
In others words, you can use this library for developement any kind of software: open source, commercial, proprietary and alien.


Change Log
======
 - 0.5.3 First version.
