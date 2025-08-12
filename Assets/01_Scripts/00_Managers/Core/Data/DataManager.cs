using Client;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Logger = Client.Logger;

public class DataManager : BaseManager
{
    const string DB_BYTES = "01_Data/DesignData";
    const string DB_NAME = "DesignData.db";
    public class DataBase
    {
        public byte[] bytes { get; set; }
        public Dictionary<string, int> IndexDic { get; set; } = new Dictionary<string, int>();
    }
    private IDbConnection _dbConnection;
    private TimeSpan _cacheExpireTime;

    // {tableName : {id : protoBytes}}
    private Dictionary<string, CacheManager<int, DataBase>> _designDataDic = new Dictionary<string, CacheManager<int, DataBase>>();

    public async override UniTask Init()
    {
        string path = await LoadDB();
        OpenConnection($"Data Source={path}");
        _cacheExpireTime = TimeSpan.FromMinutes(5);
        await UniTask.CompletedTask;
    }

    private async UniTask<string> LoadDB()
    {
        string targetPath = Path.Combine(Application.persistentDataPath, DB_NAME);

        var ta = await Managers.Resource.LoadAsync<TextAsset>(DB_BYTES);
        if(ta == null || ta.bytes.Length == 0)
        {
            throw new FileNotFoundException("DesignData.bytes not found or empty.");
        }
        File.WriteAllBytes(targetPath, ta.bytes);
        return targetPath;
    }

    public List<T> GetDataAll<T>() where T : IMessage<T>, new()
    {
        string tableName = typeof(T).Name;
        List<T> list = new List<T>();
        SelectAllData(tableName, ref list);
        return list;
    }

    public T GetDataListByIndexSingle<T>(params (string, int)[] parameters) where T : IMessage<T>, new()
    {
        List<T> list = GetDataListByIndex<T>(parameters);
        if (list.Count == 0)
            return default;

        return list[0];
    }

    public List<T> GetDataListByIndex<T>(params (string, int)[] parameters) where T : IMessage<T>, new()
    {
        string tableName = typeof(T).Name;
        List<T> list = new List<T>();

        if (_designDataDic.TryGetValue(tableName, out var tableDic) == true)
        {
            var dataBaseList = tableDic.ToList;
            foreach (var database in dataBaseList)
            {
                bool isMatch = true;
                foreach (var index in parameters)
                {
                    if (database.Value.IndexDic.TryGetValue(index.Item1, out int value) == false)
                    {
                        isMatch = false;
                        break;
                    }

                    if (value != index.Item2)
                    {
                        isMatch = false;
                        continue;
                    }
                }

                if (isMatch == false)
                    continue;

                // 캐시 데이터가 있으면 넣어줌
                T t = new T();
                t.MergeFrom(database.Value.bytes);
                list.Add(t);

                // 데이터 조회시간 갱신
                database.UpdateAccessTime();
            }
        }

        if (list.Count == 0)
        {
            SelectManyDataByIndex(tableName, ref list, parameters);
        }
        return list;
    }

    public List<T> GetAllDataListByIndex<T>(params (string, int)[] parameters) where T : IMessage<T>, new()
    {
        string tableName = typeof(T).Name;
        List<T> list = new List<T>();
        SelectManyDataByIndex(tableName, ref list, parameters);
        return list;
    }

    public T GetData<T>(int id) where T : class, IMessage<T>, new()
    {
        string tableName = typeof(T).Name;
        T t = new T();

        if (_designDataDic.TryGetValue(tableName, out var tableDic) &&
            tableDic.TryGetValue(id, out DataBase data))
        {
            t.MergeFrom(data.bytes);
        }
        else
        {
            SelectOneData(tableName, id, ref t);

            // 데이터가 여전히 없는 경우 null 반환
            if (t.CalculateSize() == 0)
            {
                Logger.LogError($"{tableName}의 {id} 존재하지 앖습니다.");
                return null;
            }
        }

        return t;
    }

    private void SelectOneData<T>(string tableName, int id, ref T t) where T : IMessage<T>, new()
    {
        using (IDbCommand dbCmd = _dbConnection.CreateCommand())
        {
            string sqlQuery = $"SELECT id, protobuf FROM {tableName} WHERE id ='{id}'";
            dbCmd.CommandText = sqlQuery;

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    byte[] protobuf = (byte[])reader["protobuf"];
                    var data = new DataBase
                    {
                        bytes = protobuf
                    };
                    CachingData(tableName, id, data);

                    // message 객체 생성 ref 반환
                    t.MergeFrom(protobuf);
                }
                reader.Close();
            }
        }
    }

    private void SelectAllData<T>(string tableName, ref List<T> list) where T : IMessage<T>, new()
    {
        using (IDbCommand dbCmd = _dbConnection.CreateCommand())
        {
            string sqlQuery = $"SELECT id, protobuf FROM {tableName}";
            dbCmd.CommandText = sqlQuery;
            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    byte[] protobuf = (byte[])reader["protobuf"];
                    var data = new DataBase
                    {
                        bytes = protobuf
                    };
                    CachingData(tableName, id, data);
                    // message 객체 생성 ref 반환
                    T t = new T();
                    t.MergeFrom(protobuf);
                    list.Add(t);
                }
                reader.Close();
            }
        }
    }

    private void SelectManyData<T>(string tableName, List<int> ids, ref List<T> list) where T : IMessage<T>, new()
    {
        using (IDbCommand dbCmd = _dbConnection.CreateCommand())
        {
            string sqlQuery = $"SELECT id, protobuf FROM {tableName} WHERE id IN ({string.Join(",", ids)})";
            dbCmd.CommandText = sqlQuery;

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    byte[] protobuf = (byte[])reader["protobuf"];
                    var data = new DataBase
                    {
                        bytes = protobuf
                    };
                    CachingData(tableName, id, data);
                    // message 객체 생성 ref 반환
                    T t = new T();
                    t.MergeFrom(protobuf);
                    list.Add(t);
                }
                reader.Close();
            }
        }
    }

    private void SelectManyDataByIndex<T>(string tableName, ref List<T> list, params (string, int)[] parameters) where T : IMessage<T>, new()
    {
        using (IDbCommand dbCmd = _dbConnection.CreateCommand())
        {
            string whereClause = "";
            Dictionary<string, int> indexDic = new Dictionary<string, int>();
            if (parameters.Length > 0)
            {
                whereClause = "WHERE ";
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    whereClause += $"{parameter.Item1} = @{parameter.Item1}";
                    if (i < parameters.Length - 1)
                    {
                        whereClause += " AND ";
                    }
                    var dbParam = dbCmd.CreateParameter();
                    dbParam.ParameterName = $"@{parameter.Item1}";
                    dbParam.Value = parameter.Item2;
                    dbCmd.Parameters.Add(dbParam);
                    indexDic.Add(parameter.Item1, parameter.Item2);
                }
            }

            string sqlQuery = $"SELECT * FROM {tableName} {whereClause}";
            dbCmd.CommandText = sqlQuery;

            using (IDataReader reader = dbCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    byte[] protobuf = (byte[])reader["protobuf"];
                    var data = new DataBase
                    {
                        bytes = protobuf
                    };
                    data.IndexDic = indexDic;
                    CachingData(tableName, id, data);
                    // message 객체 생성 ref 반환
                    T t = new T();
                    t.MergeFrom(protobuf);
                    list.Add(t);
                }
                reader.Close();
            }
        }
    }

    private void CachingData(string tableName, int id, DataBase data)
    {
        // dictionary에 캐싱
        if (_designDataDic.TryGetValue(tableName, out var cacheMgr) == false)
        {
            cacheMgr = new CacheManager<int, DataBase>(_cacheExpireTime);
            _designDataDic.Add(tableName, cacheMgr);
        }
        cacheMgr.Add(id, data);
    }

    public void OpenConnection(string connectionString)
    {
        Logger.Log("DataManager Open");
        if (_dbConnection == null)
        {
            _dbConnection = new SqliteConnection(connectionString);
            _dbConnection.Open();
        }
    }

    public void CloseConnection()
    {
        Logger.Log("DataManager Close");
        if (_dbConnection != null)
        {
            _dbConnection.Close();
            _dbConnection = null;
        }
    }

    public void Clear()
    {
        foreach (var cache in _designDataDic.Values)
        {
            cache.Clear();
        }
        _designDataDic.Clear();
    }

    public async override UniTask Dispose()
    {
        CloseConnection();
        Clear();
        await UniTask.CompletedTask;
    }
}
