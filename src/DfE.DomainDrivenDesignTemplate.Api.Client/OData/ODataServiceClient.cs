//using Default;
//using Microsoft.OData.Client;

//namespace DfE.DomainDrivenDesignTemplate.Api.Client.OData
//{
//    public class ODataServiceClient : IODataServiceClient
//    {
//        private readonly Container _container;

//        public ODataServiceClient(string serviceRoot, string token)
//        {
//            var serviceUri = new Uri(serviceRoot);
//            _container = new Container(serviceUri);

//            _container.Configurations.RequestPipeline.OnMessageCreating = (args) =>
//            {
//                var request = new HttpClientRequestMessage(args);
//                request.SetHeader("Authorization", $"Bearer {token}");
//                return request;
//            };
//        }

//        public Container Container => _container;
//    }
//}
