using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VOrb.SantaJam
{

    public static class GameStorageOperator
    {
        public struct PlayerParamNames
        {
            public const string Player = "_";
            public const string Tutorial = "_tutorial";
            public const string Level = "_level_";
            public const string LvlLock = "_lock_";
            public const string Smiles = "_smiles";
            public const string Gems = "_gems";
            public const string Sound = "_sound";
            public const string Noads = "_noads";
        }

        private static PlayerParamNames _playerParamNames = new PlayerParamNames();

        public static void PutToDevice(string tag,  object value)
        {
            var player = GameService.Instance.PlayerName;
            DataKeeper.SaveParam(player + tag, value);
        }

        public static void PutToDevice(PlayerParamNames tag, object value) => PutToDevice(tag, value);
        public static void GetFromDevice(PlayerParamNames tag, object byDefault) => GetFromDevice(tag, byDefault);
        public static object GetFromDevice(string tag, object byDefault)
        {
            var player = GameService.Instance.PlayerName;
            return DataKeeper.LoadParam(player + tag, byDefault);
        }

        public static void DropSavedPlayerInfo()
        {
            var player = GameService.Instance.PlayerName;
            foreach (var ParamInfo in _playerParamNames.GetType().GetFields())
            {
                if ((string)ParamInfo.GetValue(ParamInfo.Name) != PlayerParamNames.Noads)
                {
                    DataKeeper.DeleteKey(player + ParamInfo.GetValue(ParamInfo.Name));
                    if ((string)ParamInfo.GetValue(ParamInfo.Name) == PlayerParamNames.Level
                        || (string)ParamInfo.GetValue(ParamInfo.Name) == PlayerParamNames.LvlLock)
                    {
                        for (int i = 1; i <= DataBaseManager.Instance.LevelsInfo.Count; i++)
                        {
                            DataKeeper.DeleteKey(player + ParamInfo.GetValue(ParamInfo.Name) + i);
                        }
                    }
                }
                
            }
            
           
        }
        public static string Serialize<T>(T _object)
        {
            string tmp = JsonConvert.SerializeObject(_object);
            return tmp;
        }

        public static List<TDat> LoadJsonFromString<TDat>(string file)
        {
            TextAsset mytxtData = (TextAsset)Resources.Load(file);
            if (mytxtData == null)
            {
                return null;
            }
            string txt = mytxtData.text;

            IEnumerable<TDat> forret = JsonConvert.DeserializeObject<TDat[]>(txt);            
            return new List<TDat>(forret);
        }
    }
}