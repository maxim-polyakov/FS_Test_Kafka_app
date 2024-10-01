using Microsoft.Data.Analysis;
using System.Collections.Generic;

namespace DB_package {
    public interface IDB_Communication {
        void insert_to(string insert, string tablename);

        string get_token(string select);

        Dictionary<int, string> get_data(string select);
        bool checkcommands(string input_string);
    }
}
