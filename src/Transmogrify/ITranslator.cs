﻿using System.Threading.Tasks;

namespace Transmogrify {
    public interface ITranslator
    {
        Task<string> GetTranslation(string file, string key);
    }
}