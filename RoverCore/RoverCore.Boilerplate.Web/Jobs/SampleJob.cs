using System;
using System.Diagnostics;
using Hangfire.Annotations;
using Serviced;

namespace RoverCore.Boilerplate.Web.Jobs
{
	/// <summary>
	/// This is a sample job for Hangfire.  Note that this job is scheduled in the JobConfiguration.cs class and it's service registration is automatically
	/// performed by adding the IScoped interface.  If you fail to add this interface you will need to register the service manually in Startup.cs
	/// </summary>
    public class SampleJob : IScoped
    {
	    public void Hello()
	    {
			Debug.WriteLine("Hangfire job -- SampleJob -- Hello world!");
	    }
    }
}
