using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Session
{
    public class PsXmlRepository : IXmlRepository
    {
        private static IAmazonSimpleSystemsManagement _client;

        public PsXmlRepository(IAmazonSimpleSystemsManagement client)
        {
            _client = client;
        }
		
		//GetAllElementsは、Parameter StoreのGetParametersByPathAsyncメソッドを使用して、「CookieEncryptionKey」で始まるすべてのパラメーターを取得し、XElementのコレクションとして返します。
		public IReadOnlyCollection<XElement> GetAllElements()
        {
            var request = new GetParametersByPathRequest
            {
                Path = "/CookieEncryptionKey"
            };

            var response = _client.GetParametersByPathAsync(request).Result;
            var result = new List<XElement>(response.Parameters.Count);

            response.Parameters.ForEach(x => result.Add(XElement.Parse(x.Value)));
            
            return result;
        }

		// GetAllElements()でCookieEncryptionKeyがAWSのSSMにパラメータが無かったら作成する。
		// StoreElementはXElementを受け取り、「CookieEncryptionKey」に設定された名前とミドルウェアから渡されたフレンドリ名（GUID）をパラメーターとしてパラメーターストアに格納します。
		public void StoreElement(XElement element, string friendlyName)
        {
			var request = new PutParameterRequest
            {
                Name = "/CookieEncryptionKey/" + friendlyName,
                Value = element.ToString(),
                Description = "Key-" + friendlyName,
                Overwrite = true,
                Type = ParameterType.String
            };

            _client.PutParameterAsync(request);
        }
    }
}
