using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.OSS
{
    public class AliyunOSSProvider : IOSSProvider
    {
        public ValueTask<bool> Delete(OSSOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetFileUrl(OSSOptions options)
        {
            throw new NotImplementedException();
        }

    }
}
