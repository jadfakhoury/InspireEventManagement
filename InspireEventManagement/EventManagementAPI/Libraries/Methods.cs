using BookstopNetModels.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagementAPI.Methods
{
    public class Methods
    {
        private readonly ILogger<Methods> _logger;
        public IConfiguration Configuration { get; }
        public Methods(ILogger<Methods> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }
        public async Task<List<string>> UploadPhotos(IEnumerable<IFormFile> files)
        {
            string guid = "";
            List<string> guidList = new List<string>();
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            guid = Guid.NewGuid().ToString();
                            string fileName = guid + ".jpeg";
                            string filePath = Path.Combine(Configuration["VirtualDir"], fileName);
                            long size = files.Sum(f => f.Length);

                            if (file.Length > 0)
                            {
                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                guidList.Add(fileName);
                            }
                        }
                    }
                }
                return guidList;
            }
            catch (Exception e)
            {
                ResponseModel error = new ResponseModel(e);
                _logger.LogError("\nSource: GenericCRUD\n" + error.ToString());
                throw;
            }
        }
    }
}
