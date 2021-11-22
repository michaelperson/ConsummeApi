using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequesterTool.Interfaces
{
    public interface IRequester
    {        
        Task<List<T>> Get<T>(string route);
        Task<bool> Post<T>(string route, T element);
        Task<bool> Put<T>(string route, T element);
        Task<bool> Delete<T>(string route, T identifiant);
        Task<bool> Patch<T,U>(string route, U identifiant, JsonPatchDocument<T> patchDoc)
            where T : class;
    }
}
