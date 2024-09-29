namespace DfE.DomainDrivenDesignTemplate.Application.Services.BackgroundServices.Tasks
{
    public class CreateReportExampleTask
    {
        public async Task<string> RunAsync(string taskName)
        {
            await Task.Delay(10000);

            return $"Task: {taskName} completed after 10 seconds.";
        }
    }
}
