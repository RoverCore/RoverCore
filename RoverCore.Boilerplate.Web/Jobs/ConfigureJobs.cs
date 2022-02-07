using Hangfire;

namespace RoverCore.Boilerplate.Web.Jobs
{
    public static class ConfigureJobs
    {
        /// <summary>
        /// Schedule all your recurring Hangfire jobs in this method.  This will be called at the end of the application middleware initialization.
        /// </summary>
	    public static void Schedule()
	    {
            // Sample job that prints hello every minute to the console
		    RecurringJob.AddOrUpdate<SampleJob>(generator => generator.Hello(), "* * * * *");
	    }
    }
}
